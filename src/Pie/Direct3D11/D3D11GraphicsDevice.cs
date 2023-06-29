using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pie.DebugLayer;
using Pie.ShaderCompiler;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11GraphicsDevice : GraphicsDevice
{
    public static D3D11 D3D11;
    public static DXGI DXGI;
    public static D3DCompiler D3DCompiler;

    private Silk.NET.DXGI.Format _colorFormat;
    private Silk.NET.DXGI.Format? _depthFormat;
    
    public static ComPtr<ID3D11Device> Device;
    public static ComPtr<ID3D11DeviceContext> Context;
    
    private ComPtr<IDXGIFactory2> _dxgiFactory;
    private ComPtr<IDXGISwapChain> _swapChain;
    private ComPtr<ID3D11Texture2D> _colorTexture;
    private ComPtr<ID3D11Texture2D> _depthStencilTexture;
    private ComPtr<ID3D11RenderTargetView> _colorTargetView;
    private ComPtr<ID3D11DepthStencilView> _depthStencilTargetView;
    
    private InputLayout _currentLayout;
    private RasterizerState _currentRState;
    private BlendState _currentBState;
    private DepthStencilState _currentDStencilState;
    private PrimitiveType _currentPType;
    private bool _primitiveTypeInitialized;

    private D3D11Framebuffer _currentFramebuffer;

    public unsafe D3D11GraphicsDevice(IntPtr hwnd, Size winSize, GraphicsDeviceOptions options)
    {
        DXGI = DXGI.GetApi(null);
        D3D11 = D3D11.GetApi(null);
        D3DCompiler = D3DCompiler.GetApi();

        bool debug = options.Debug;
        if (debug && !CheckDebugSdk(D3D11))
        {
            debug = false;
            PieLog.Log(LogType.Warning, "Debug has been enabled however no SDK layers have been found. Direct3D debug has therefore been disabled.");
        }
        
        if (!Succeeded(DXGI.CreateDXGIFactory2(debug ? (uint) DXGI.CreateFactoryDebug : 0, out _dxgiFactory)))
            throw new PieException("Failed to create DXGI factory.");

        D3DFeatureLevel level = D3DFeatureLevel.Level110;

        CreateDeviceFlag flags = CreateDeviceFlag.BgraSupport | CreateDeviceFlag.Singlethreaded;
        if (debug)
            flags |= CreateDeviceFlag.Debug;

        _colorFormat = options.ColorBufferFormat.ToDxgiFormat(false);
        _depthFormat = options.DepthStencilBufferFormat?.ToDxgiFormat(false);

        SwapChainDesc swapChainDescription = new SwapChainDesc()
        {
            Flags = (uint) SwapChainFlag.AllowTearing | (uint) SwapChainFlag.AllowModeSwitch,
            BufferCount = 2,
            BufferDesc = new ModeDesc((uint) winSize.Width, (uint) winSize.Height, format: _colorFormat),
            BufferUsage = DXGI.UsageRenderTargetOutput,
            OutputWindow = hwnd,
            SampleDesc = new SampleDesc(1, 0),
            SwapEffect = SwapEffect.FlipDiscard,
            Windowed = true
        };

        ComPtr<IDXGIAdapter> adapter = null;
        _dxgiFactory!.EnumAdapters(0, ref adapter);
        AdapterDesc desc;
        adapter.GetDesc(&desc);
        
        Adapter = new GraphicsAdapter(Marshal.PtrToStringAnsi((IntPtr) desc.Description));

        if (!Succeeded(D3D11.CreateDeviceAndSwapChain(new ComPtr<IDXGIAdapter>((IDXGIAdapter*) null), D3DDriverType.Hardware, 0,
                (uint) flags, &level, 1, D3D11.SdkVersion, &swapChainDescription, ref _swapChain, ref Device, null,
                ref Context)))
        {
            throw new PieException("Failed to create device or swapchain.");
        }

        if (!Succeeded(_swapChain.GetBuffer(0, out _colorTexture)))
            throw new PieException("Failed to get the back color buffer.");

        if (!Succeeded(Device.CreateRenderTargetView(_colorTexture, null, ref _colorTargetView)))
            throw new PieException("Failed to create swapchain color target.");
        CreateDepthStencilView(winSize);
        
        Viewport = new Rectangle(Point.Empty, winSize);
        Swapchain = new Swapchain()
        {
            Size = winSize
        };

        Context.OMSetRenderTargets(1, ref _colorTargetView, _depthStencilTargetView);
    }

    public override GraphicsApi Api => GraphicsApi.D3D11;
    public override Swapchain Swapchain { get; }
    public override GraphicsAdapter Adapter { get; }

    private Rectangle _viewport;
    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            Silk.NET.Direct3D11.Viewport viewport = new Viewport(value.X, value.Y, value.Width, value.Height, 0f, 1f);
            Context.RSSetViewports(1, viewport);
        }
    }

    private Rectangle _scissor;

    public override Rectangle Scissor
    {
        get => _scissor;
        set
        {
            _scissor = value;

            Silk.NET.Maths.Box2D<int> scissor =
                new Silk.NET.Maths.Box2D<int>(value.X, value.Y, value.Width, value.Height);
            Context.RSSetScissorRects(1, scissor);
        }
    }

    public override void ClearColorBuffer(Color color)
    {
        ClearColorBuffer(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
    }

    public override void ClearColorBuffer(Vector4 color)
    {
        Context.ClearRenderTargetView(_currentFramebuffer?.Targets[0] ?? _colorTargetView, &color.X);
    }

    public override void ClearDepthStencilBuffer(ClearFlags flags, float depth, byte stencil)
    {
        //Context.RSSetViewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
        uint cf = 0;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            cf |= (uint) ClearFlag.Depth;

        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            cf |= (uint) ClearFlag.Stencil;

        Context.ClearDepthStencilView(_currentFramebuffer?.DepthStencil ?? _depthStencilTargetView, cf, depth, stencil);
    }

    private void InvalidateCache()
    {
        _currentLayout = null;
        _currentRState = null;
        _currentBState = null;
        _currentDStencilState = null;
    }

    public override unsafe GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        fixed (void* dat = data) 
            return new D3D11GraphicsBuffer(bufferType, (uint) (data.Length * Unsafe.SizeOf<T>()), dat, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        fixed (void* dat = new T[] { data })
            return new D3D11GraphicsBuffer(bufferType, (uint) Unsafe.SizeOf<T>(), dat, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(bufferType, sizeInBytes, null, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, IntPtr data, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(bufferType, sizeInBytes, data.ToPointer(), dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, void* data, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(bufferType, sizeInBytes, data, dynamic);
    }

    public override unsafe Texture CreateTexture(TextureDescription description)
    {
        return new D3D11Texture(description, null);
    }

    public override unsafe Texture CreateTexture<T>(TextureDescription description, T[] data)
    {
        fixed (void* ptr = data)
            return new D3D11Texture(description, ptr);
    }

    public override unsafe Texture CreateTexture<T>(TextureDescription description, T[][] data)
    {
        fixed (void* ptr = PieUtils.Combine(data))
            return new D3D11Texture(description, ptr);
    }

    public override unsafe Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        return new D3D11Texture(description, data.ToPointer());
    }

    public override unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        return new D3D11Texture(description, data);
    }

    public override Shader CreateShader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        return new D3D11Shader(attachments, constants);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new D3D11InputLayout(descriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new D3D11RasterizerState(description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new D3D11BlendState(description);
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new D3D11DepthStencilState(description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new D3D11SamplerState(description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new D3D11Framebuffer(attachments);
    }

    public override unsafe void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        fixed (void* dat = data)
            ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, (uint) (data.Length * Unsafe.SizeOf<T>()), dat);
    }

    public override unsafe void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        fixed (void* dat = new T[] { data })
            ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, (uint) Unsafe.SizeOf<T>(), dat);
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, sizeInBytes, data.ToPointer());
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, sizeInBytes, data);
    }

    public override unsafe void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z,
        int width, int height, int depth, T[] data)
    {
        fixed (void* dat = data)
            ((D3D11Texture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, dat);
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width,
        int height, int depth, IntPtr data)
    {
        ((D3D11Texture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, data.ToPointer());
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z,
        int width, int height, int depth, void* data)
    {
        ((D3D11Texture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, data);
    }

    public override MappedSubresource MapResource(GraphicsResource resource, MapMode mode)
    {
        return resource.Map(mode);
    }

    public override void UnmapResource(GraphicsResource resource)
    {
        resource.Unmap();
    }

    public override void SetShader(Shader shader)
    {
        D3D11Shader sh = (D3D11Shader) shader;
        sh.Use();
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState state)
    {
        D3D11Texture tex = (D3D11Texture) texture;
        Context.PSSetShaderResources(bindingSlot, 1, ref tex.View);
        Context.PSSetSamplers(bindingSlot, 1, ref ((D3D11SamplerState) state).State);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        //if (state == _currentRState)
        //    return;
        _currentRState = state;
        Context.RSSetState(((D3D11RasterizerState) state).State);
    }

    public override void SetBlendState(BlendState state)
    {
        //if (state == _currentBState)
        //    return;
        _currentBState = state;
        Context.OMSetBlendState(((D3D11BlendState) state).State, null, uint.MaxValue);
    }

    public override void SetDepthStencilState(DepthStencilState state, int stencilRef)
    {
        //if (state == _currentDStencilState)
        //    return;
        _currentDStencilState = state;
        Context.OMSetDepthStencilState(((D3D11DepthStencilState) state).State, (uint) stencilRef);
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        //if (_primitiveTypeInitialized && type == _currentPType)
        //    return;
        _primitiveTypeInitialized = true;
        _currentPType = type;
        D3DPrimitiveTopology topology = type switch
        {
            PrimitiveType.TriangleList => D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglelist,
            PrimitiveType.TriangleStrip => D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglestrip,
            PrimitiveType.LineList => D3DPrimitiveTopology.D3DPrimitiveTopologyLinelist,
            PrimitiveType.LineStrip => D3DPrimitiveTopology.D3DPrimitiveTopologyLinestrip,
            PrimitiveType.PointList => D3DPrimitiveTopology.D3DPrimitiveTopologyPointlist,
            PrimitiveType.TriangleListAdjacency => D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglelistAdj,
            PrimitiveType.TriangleStripAdjacency => D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglestripAdj,
            PrimitiveType.LineListAdjacency => D3DPrimitiveTopology.D3DPrimitiveTopologyLinelistAdj,
            PrimitiveType.LineStripAdjacency => D3DPrimitiveTopology.D3DPrimitiveTopologyLinestripAdj,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        Context.IASetPrimitiveTopology(topology);
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
        //if (layout != _currentLayout)
        //{
        //    _currentLayout = layout;
            D3D11InputLayout lt = (D3D11InputLayout) layout;
            Context.IASetInputLayout(lt.Layout);
        //}

        Context.IASetVertexBuffers(slot, 1, ref ((D3D11GraphicsBuffer) buffer).Buffer, stride, 0);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        Silk.NET.DXGI.Format fmt = type switch
        {
            IndexType.UShort => Silk.NET.DXGI.Format.FormatR16Uint,
            IndexType.UInt => Silk.NET.DXGI.Format.FormatR32Uint,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        Context.IASetIndexBuffer(((D3D11GraphicsBuffer) buffer).Buffer, fmt, 0);
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        D3D11GraphicsBuffer buf = (D3D11GraphicsBuffer) buffer;
        Context.VSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
        Context.PSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
        Context.GSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
        Context.CSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        _currentFramebuffer = (D3D11Framebuffer) framebuffer;
        if (framebuffer == null)
            Context.OMSetRenderTargets(1, ref _colorTargetView, _depthStencilTargetView);
        else
        {
            Context.OMSetRenderTargets((uint) _currentFramebuffer.Targets.Length, ref _currentFramebuffer.Targets[0],
                _currentFramebuffer.DepthStencil);
        }

    }

    public override void Draw(uint vertexCount)
    {
        Context.Draw(vertexCount, 0);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += vertexCount / 3;
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
        Context.Draw(vertexCount, (uint) startVertex);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += vertexCount/ 3;
    }

    public override void DrawIndexed(uint indexCount)
    {
        Context.DrawIndexed(indexCount, 0, 0);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount / 3;
    }

    public override void DrawIndexed(uint indexCount, int startIndex)
    {
        Context.DrawIndexed(indexCount, (uint) startIndex, 0);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount/ 3;
    }

    public override void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        Context.DrawIndexed(indexCount, (uint) startIndex, baseVertex);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount/ 3;
    }

    public override void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        Context.DrawIndexedInstanced( indexCount, instanceCount, 0, 0, 0);
        PieMetrics.TriCount += indexCount / 3 * instanceCount;
        PieMetrics.DrawCalls++;
    }

    public override void Present(int swapInterval)
    {
        uint flags = 0;

        if (swapInterval == 0)
            flags |= DXGI.PresentAllowTearing;
        _swapChain.Present((uint) swapInterval, flags);
        // ?????? This only seems to happen on AMD but after presentation the render targets go off to floaty land
        // I'm sure usually this is resolved by setting render targets at the start of a frame (like what Easel does)
        // but Pie has no way of knowing when the start of a frame is, so just do it at the end of presentation.
        Context.OMSetRenderTargets(1, ref _colorTargetView, _depthStencilTargetView);
        
        PieMetrics.DrawCalls = 0;
        PieMetrics.TriCount = 0;
    }

    public override void ResizeSwapchain(Size newSize)
    {
        Context.Flush();
        Context.OMSetRenderTargets(1, (ID3D11RenderTargetView*) null, (ID3D11DepthStencilView*) null);
        _colorTargetView.Dispose();
        _colorTexture.Dispose();

        if (_depthFormat != null)
        {
            _depthStencilTargetView.Dispose();
            _depthStencilTexture.Dispose();
        }

        _swapChain.ResizeBuffers(0, (uint) newSize.Width, (uint) newSize.Height, Silk.NET.DXGI.Format.FormatUnknown, 0);
        _colorTexture = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        if (!Succeeded(Device.CreateRenderTargetView(_colorTexture, null, ref _colorTargetView)))
            throw new PieException("Failed to create swapchain color target.");
        CreateDepthStencilView(newSize);

        Swapchain.Size = newSize;
    }

    public override void GenerateMipmaps(Texture texture)
    {
        Context.GenerateMips(((D3D11Texture) texture).View);
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        Context.Dispatch(groupCountX, groupCountY, groupCountZ);
    }

    public override void Flush()
    {
        Context.Flush();
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
        if (_depthFormat == null)
            return;
        
        Texture2DDesc texDesc = new Texture2DDesc()
        {
            Format = _depthFormat.Value,
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            ArraySize = 1,
            MipLevels = 1,
            BindFlags = (uint) BindFlag.DepthStencil,
            CPUAccessFlags = (uint) CpuAccessFlag.None,
            MiscFlags = (uint) ResourceMiscFlag.None,
            SampleDesc = new SampleDesc(1, 0),
            Usage = Usage.Default
        };

        DepthStencilViewDesc viewDesc = new DepthStencilViewDesc()
        {
            Format = texDesc.Format,
            ViewDimension = DsvDimension.Texture2D
        };

        if (!Succeeded(Device.CreateTexture2D(&texDesc, null, ref _depthStencilTexture)))
            throw new PieException("Failed to create swapchain depth texture.");

        if (!Succeeded(Device.CreateDepthStencilView(_depthStencilTexture, &viewDesc, ref _depthStencilTargetView)))
            throw new PieException("Failed to create swapchain depth view.");
    }
}