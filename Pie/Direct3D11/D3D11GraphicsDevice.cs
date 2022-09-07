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

internal sealed class D3D11GraphicsDevice : GraphicsDevice
{
    private IDXGIFactory2 _dxgiFactory;
    public static ID3D11Device Device;
    public static ID3D11DeviceContext Context;
    private IDXGISwapChain _swapChain;
    private ID3D11Texture2D _colorTexture;
    private ID3D11Texture2D _depthStencilTexture;
    private ID3D11RenderTargetView _colorTargetView;
    private ID3D11DepthStencilView _depthStencilTargetView;
    
    private InputLayout _currentLayout;
    private RasterizerState _currentRState;
    private BlendState _currentBState;
    private DepthState _currentDState;
    private PrimitiveType _currentPType;
    private bool _primitiveTypeInitialized;

    private D3D11Framebuffer _currentFramebuffer;

    public D3D11GraphicsDevice(IntPtr hwnd, Size winSize, GraphicsDeviceOptions options)
    {
        bool debug = options.Debug;
        if (debug && !SdkLayersAvailable())
        {
            debug = false;
            Logging.Log(LogType.Warning, "Debug has been enabled however no SDK layers have been found. Direct3D debug has therefore been disabled.");
        }
        
        Result res;
        if ((res = CreateDXGIFactory2(debug, out _dxgiFactory)).Failure)
            throw new PieException("Error creating DXGI factory: " + res.Description);

        FeatureLevel[] levels = new[]
        {
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_11_1
        };

        DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Singlethreaded;
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

        if ((res = _swapChain!.GetBuffer(0, out _colorTexture)).Failure)
            throw new PieException("Failed to get the back buffer: " + res.Description);

        _colorTargetView = Device!.CreateRenderTargetView(_colorTexture);
        CreateDepthStencilView(winSize);
        
        Viewport = new Rectangle(Point.Empty, winSize);
        
        SetFramebuffer(null);
    }

    public override GraphicsApi Api => GraphicsApi.D3D11;
    
    private Rectangle _viewport;

    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            Context.RSSetViewport(value.X, value.Y, value.Width, value.Height);
        }
    }

    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        Clear(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
        Clear(flags);
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
        Context.ClearRenderTargetView(_currentFramebuffer?.Targets[0] ?? _colorTargetView, new Color4(color));
        Clear(flags);
    }

    public override void Clear(ClearFlags flags)
    {
        Context.RSSetViewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
        DepthStencilClearFlags cf = DepthStencilClearFlags.None;
        if (flags.HasFlag(ClearFlags.Depth))
            cf |= DepthStencilClearFlags.Depth;
        if (flags.HasFlag(ClearFlags.Stencil))
            cf |= DepthStencilClearFlags.Stencil;
        Context.ClearDepthStencilView(_currentFramebuffer?.DepthStencil ?? _depthStencilTargetView, cf, 1, 0);
        //Context.OMSetRenderTargets(_colorTargetView, _depthStencilTargetView);
    }

    private void InvalidateCache()
    {
        _currentLayout = null;
        _currentRState = null;
        _currentBState = null;
        _currentDState = null;
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

    public override Texture CreateTexture<T>(TextureDescription description, T[] data = null)
    {
        return D3D11Texture.CreateTexture(description, data);
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new D3D11Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new D3D11InputLayout(descriptions);
    }

    public override InputLayout CreateInputLayout(uint stride, params InputLayoutDescription[] descriptions)
    {
        return new D3D11InputLayout(stride, descriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new D3D11RasterizerState(description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new D3D11BlendState(description);
    }

    public override DepthState CreateDepthState(DepthStateDescription description)
    {
        return new D3D11DepthState(description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new D3D11SamplerState(description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new D3D11Framebuffer(attachments);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, data);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, data);
    }

    public override void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data)
    {
        ((D3D11Texture) texture).Update(x, y, width, height, data);
    }

    public override void SetShader(Shader shader)
    {
        D3D11Shader sh = (D3D11Shader) shader;
        sh.Use();
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState state)
    {
        D3D11Texture tex = (D3D11Texture) texture;
        Context.PSSetShaderResource((int) bindingSlot, tex.View);
        Context.PSSetSampler((int) bindingSlot, ((D3D11SamplerState) state).State);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        if (state == _currentRState)
            return;
        _currentRState = state;
        Context.RSSetState(((D3D11RasterizerState) state).State);
    }

    public override void SetBlendState(BlendState state)
    {
        if (state == _currentBState)
            return;
        _currentBState = state;
        Context.OMSetBlendState(((D3D11BlendState) state).State);
    }

    public override void SetDepthState(DepthState state)
    {
        if (state == _currentDState)
            return;
        _currentDState = state;
        Context.OMSetDepthStencilState(((D3D11DepthState) state).State);
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        if (_primitiveTypeInitialized && type == _currentPType)
            return;
        _primitiveTypeInitialized = true;
        _currentPType = type;
        PrimitiveTopology topology = type switch
        {
            PrimitiveType.TriangleList => PrimitiveTopology.TriangleList,
            PrimitiveType.TriangleStrip => PrimitiveTopology.TriangleStrip,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        Context.IASetPrimitiveTopology(topology);
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        if (layout != _currentLayout)
        {
            _currentLayout = layout;
            D3D11InputLayout lt = (D3D11InputLayout) layout;
            Context.IASetInputLayout(lt.Layout);
        }

        Context.IASetVertexBuffer(0, ((D3D11GraphicsBuffer) buffer).Buffer, (int) _currentLayout.Stride);
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

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        _currentFramebuffer = (D3D11Framebuffer) framebuffer;
        if (framebuffer == null)
            Context.OMSetRenderTargets(_colorTargetView, _depthStencilTargetView);
        else
        {
            Context.OMSetRenderTargets(_currentFramebuffer.Targets, _currentFramebuffer.DepthStencil);
        }

    }

    public override void Draw(uint elements)
    {
        Context.DrawIndexed((int) elements, 0, 0);
    }

    public override void Present(int swapInterval)
    {
        _swapChain.Present(swapInterval, PresentFlags.None);
    }

    public override void ResizeSwapchain(Size newSize)
    {
        Context.UnsetRenderTargets();
        _colorTargetView.Dispose();
        _depthStencilTargetView.Dispose();
        _colorTexture.Dispose();
        _depthStencilTexture.Dispose();
        
        _swapChain.ResizeBuffers(0, newSize.Width, newSize.Height);
        _colorTexture = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        _colorTargetView = Device.CreateRenderTargetView(_colorTexture);
        CreateDepthStencilView(newSize);
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

    private void CreateDepthStencilView(Size size)
    {
        Texture2DDescription texDesc = new Texture2DDescription()
        {
            Format = Format.D32_Float,
            Width = size.Width,
            Height = size.Height,
            ArraySize = 1,
            MipLevels = 1,
            BindFlags = BindFlags.DepthStencil,
            CPUAccessFlags = CpuAccessFlags.None,
            MiscFlags = ResourceOptionFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default
        };
        
        _depthStencilTexture = Device.CreateTexture2D(Format.D24_UNorm_S8_UInt, size.Width, size.Height, 1, 1, null, BindFlags.DepthStencil);
        _depthStencilTargetView = Device.CreateDepthStencilView(_depthStencilTexture,
            new DepthStencilViewDescription(_depthStencilTexture, DepthStencilViewDimension.Texture2D));
    }
}