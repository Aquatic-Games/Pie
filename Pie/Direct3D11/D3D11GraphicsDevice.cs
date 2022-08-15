using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct3D11.Debug;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Vortice.DXGI.DXGI;
using static Vortice.Direct3D11.D3D11;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

namespace Pie.Direct3D11;

internal class D3D11GraphicsDevice : GraphicsDevice
{
    private IDXGIFactory2 _dxgiFactory;
    public static ID3D11Device Device;
    public static ID3D11DeviceContext Context;
    private IDXGISwapChain _swapChain;
    private ID3D11Texture2D _colorTexture;
    private ID3D11RenderTargetView _colorTargetView;

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
        
        Console.WriteLine(flags);

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

        if ((res = _swapChain.GetBuffer(0, out _colorTexture)).Failure)
            throw new PieException("Failed to get the back buffer: " + res.Description);

        _colorTargetView = Device.CreateRenderTargetView(_colorTexture);
        
        Viewport = new Rectangle(Point.Empty, winSize);
        VSync = vsync;
    }

    public override GraphicsApi Api => GraphicsApi.D3D11;
    
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
        Context.ClearRenderTargetView(_colorTargetView, new Color4(color));
        Context.RSSetViewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
        Context.OMSetRenderTargets(_colorTargetView);
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

    public override Texture CreateTexture<T>(int width, int height, PixelFormat format, T[] data, TextureSample sample = TextureSample.Linear, bool mipmap = true)
    {
        return D3D11Texture.CreateTexture(width, height, format, data, sample, mipmap);
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new D3D11Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new D3D11InputLayout(descriptions);
    }

    public override RasterizerState CreateRasterizerState(CullFace face = CullFace.Back, CullDirection direction = CullDirection.Clockwise, FillMode fillMode = FillMode.Solid, bool enableScissor = false)
    {
        return new D3D11RasterizerState(face, direction, fillMode, enableScissor);
    }

    public override void SetShader(Shader shader)
    {
        D3D11Shader sh = (D3D11Shader) shader;
        sh.Use();
    }

    public override void SetTexture(uint bindingSlot, Texture texture)
    {
        D3D11Texture tex = (D3D11Texture) texture;
        Context.PSSetShaderResource((int) bindingSlot, tex.View);
        Context.PSSetSampler((int) bindingSlot, tex.SamplerState);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        Context.RSSetState(((D3D11RasterizerState) state).State);
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

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        D3D11GraphicsBuffer buf = (D3D11GraphicsBuffer) buffer;
        Context.VSSetConstantBuffer((int) bindingSlot, buf.Buffer);
        Context.PSSetConstantBuffer((int) bindingSlot, buf.Buffer);
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
        Context.UnsetRenderTargets();
        _colorTargetView.Dispose();
        _colorTexture.Dispose();
        
        _swapChain.ResizeBuffers(0, newSize.Width, newSize.Height);
        _colorTexture = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        _colorTargetView = Device.CreateRenderTargetView(_colorTexture);
        Viewport = new Rectangle(Point.Empty, newSize);
    }

    public override void Dispose()
    {
        _colorTargetView.Dispose();
        _swapChain.Dispose();
        Device.Dispose();
        Context.Dispose();
        _dxgiFactory.Dispose();
    }
}