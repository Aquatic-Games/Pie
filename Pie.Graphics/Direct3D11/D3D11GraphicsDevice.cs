using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Vortice.DXGI.DXGI;
using static Vortice.Direct3D11.D3D11;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

namespace Pie.Graphics.Direct3D11;

internal class D3D11GraphicsDevice : GraphicsDevice
{
    private IDXGIFactory2 _dxgiFactory;
    public static ID3D11Device Device;
    public static ID3D11DeviceContext Context;
    private IDXGISwapChain _swapChain;
    private ID3D11RenderTargetView _renderTarget;
    
    public D3D11GraphicsDevice(IntPtr hwnd, Size winSize, bool vsync, bool debug)
    {
        if (debug && !SdkLayersAvailable())
        {
            debug = false;
            Logging.Log("WARNING: Debug has been enabled however no SDK layers have been found. Direct3D debug has therefore been disabled.");
        }
        
        Result res;
        if ((res = CreateDXGIFactory2(debug, out _dxgiFactory)).Failure)
            throw new PieException("Error creating DXGI factory: " + res.Description);

        FeatureLevel[] levels = new[]
        {
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_11_1
        };

        DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport;
        if (debug)
            flags |= DeviceCreationFlags.Debug;

        SwapChainDescription swapChainDescription = new SwapChainDescription()
        {
            Flags = SwapChainFlags.None,
            BufferCount = 2,
            BufferDescription = new ModeDescription(winSize.Width, winSize.Height),
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = hwnd,
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.FlipDiscard,
            Windowed = true
        };

        if ((res = D3D11CreateDeviceAndSwapChain(null, DriverType.Hardware, flags, levels, swapChainDescription,
                out _swapChain, out Device, out _, out Context)).Failure)
        {
            throw new PieException("Failed to create device or swapchain: " + res.Description);
        }

        if ((res = _swapChain.GetBuffer(0, out ID3D11Texture2D backBuffer)).Failure)
            throw new PieException("Failed to get the back buffer: " + res.Description);

        _renderTarget = Device.CreateRenderTargetView(backBuffer);
        
        Viewport = new Rectangle(Point.Empty, winSize);
        VSync = vsync;
    }

    public override GraphicsApi Api => GraphicsApi.D3D11;
    
    public override RasterizerState RasterizerState { get; set; }
    public override DepthMode DepthMode { get; set; }
    public override Rectangle Viewport { get; set; }

    private int _swapInterval;

    public override bool VSync
    {
        get => _swapInterval == 1;
        set => _swapInterval = value ? 1 : 0;
    }

    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        Clear(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
        Context.ClearRenderTargetView(_renderTarget, new Color4(color));
        Context.RSSetViewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
        Context.OMSetRenderTargets(_renderTarget);
    }

    public override void Clear(ClearFlags flags)
    {
        throw new System.NotImplementedException();
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        return D3D11GraphicsBuffer.CreateBuffer(bufferType, (uint) (data.Length * Unsafe.SizeOf<T>()), data, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        return D3D11GraphicsBuffer.CreateBuffer(bufferType, (uint) Unsafe.SizeOf<T>(), new T[] { data }, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T[] data, bool dynamic = false)
    {
        return D3D11GraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, data, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T data, bool dynamic = false)
    {
        return D3D11GraphicsBuffer.CreateBuffer(bufferType, sizeInBytes, new T[] { data }, dynamic);
    }

    public override Texture CreateTexture<T>(uint width, uint height, PixelFormat format, T[] data, TextureSample sample = TextureSample.Linear, bool mipmap = true)
    {
        throw new System.NotImplementedException();
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new D3D11Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new D3D11InputLayout(descriptions);
    }

    public override void SetShader(Shader shader)
    {
        D3D11Shader sh = (D3D11Shader) shader;
        sh.Use();
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        throw new System.NotImplementedException();
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        D3D11InputLayout lt = (D3D11InputLayout) layout;
        Context.IASetInputLayout(lt.Layout);
        Context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        Context.IASetVertexBuffer(0, ((D3D11GraphicsBuffer) buffer).Buffer, lt.Stride);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer)
    {
        Context.IASetIndexBuffer(((D3D11GraphicsBuffer) buffer).Buffer, Format.R32_UInt, 0);
    }

    public override void SetUniformBuffer(ShaderStage stage, uint slot, GraphicsBuffer buffer)
    {
        // I don't like this solution but it's the only one I can think of right now.
        switch (stage)
        {
            case ShaderStage.Vertex:
                Context.VSSetConstantBuffer((int) slot, ((D3D11GraphicsBuffer) buffer).Buffer);
                break;
            case ShaderStage.Fragment:
                Context.PSSetConstantBuffer((int) slot, ((D3D11GraphicsBuffer) buffer).Buffer);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
        }
    }

    public override void Draw(uint elements)
    {
        Context.DrawIndexed((int) elements, 0, 0);
    }

    public override void Present()
    {
        _swapChain.Present(_swapInterval, PresentFlags.None);
    }

    public override void ResizeMainFramebuffer(Size newSize)
    {
        Context.Flush();
        _renderTarget.Dispose();
        _swapChain.ResizeBuffers(0, newSize.Width, newSize.Height);
        _renderTarget = Device.CreateRenderTargetView(_swapChain.GetBuffer<ID3D11Texture2D>(0));
    }

    public override void Dispose()
    {
        _renderTarget.Dispose();
        _swapChain.Dispose();
        Device.Dispose();
        Context.Dispose();
        _dxgiFactory.Dispose();
    }
}