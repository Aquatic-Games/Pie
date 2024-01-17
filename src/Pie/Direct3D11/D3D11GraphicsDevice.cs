using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Pie.ShaderCompiler;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using DxgiFormat = Vortice.DXGI.Format;
using static Vortice.Direct3D11.D3D11;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11GraphicsDevice : GraphicsDevice
{
    private DxgiFormat _colorFormat;
    private DxgiFormat? _depthFormat;
    
    private ID3D11Device _device;
    private ID3D11DeviceContext _context;
    
    private IDXGIFactory2 _dxgiFactory;
    private IDXGISwapChain _swapChain;
    private ID3D11Texture2D _colorTexture;
    private ID3D11Texture2D _depthStencilTexture;
    private ID3D11RenderTargetView _colorTargetView;
    private ID3D11DepthStencilView _depthStencilTargetView;

    private D3D11Framebuffer _currentFramebuffer;

    public D3D11GraphicsDevice(IntPtr hwnd, Size winSize, GraphicsDeviceOptions options)
    {
        bool debug = options.Debug;
        if (debug && !SdkLayersAvailable())
        {
            debug = false;
            PieLog.Log(LogType.Warning, "Debug has been enabled however no SDK layers have been found. Direct3D debug has therefore been disabled.");
        }

        FeatureLevel[] levels = new [] { FeatureLevel.Level_11_0 };

        DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Singlethreaded;
        if (debug)
            flags |= DeviceCreationFlags.Debug;

        _colorFormat = options.ColorBufferFormat.ToDxgiFormat(false);
        _depthFormat = options.DepthStencilBufferFormat?.ToDxgiFormat(false);

        SwapChainDescription swapChainDescription = new SwapChainDescription()
        {
            Flags = SwapChainFlags.AllowTearing | SwapChainFlags.AllowModeSwitch,
            BufferCount = 2,
            BufferDescription = new ModeDescription(winSize.Width, winSize.Height, format: _colorFormat),
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = hwnd,
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.FlipDiscard,
            Windowed = true
        };
        
        // TODO: Adapter.
        //Adapter = new GraphicsAdapter(new string(desc.Description));

        Result result;
        
        if ((result = D3D11CreateDeviceAndSwapChain(null, DriverType.Hardware, flags, levels, swapChainDescription,
                out _swapChain, out _device, out _, out _context)).Failure)
        {
            throw new PieException("Failed to create D3D11 device: " + result.Description);
        }

        if ((result = _swapChain!.GetBuffer(0, out _colorTexture)).Failure)
            throw new PieException("Failed to get the back color buffer. " + result.Description);

        _colorTargetView = _device!.CreateRenderTargetView(_colorTexture, null);

        CreateDepthStencilView(winSize);
        
        Viewport = new Rectangle(Point.Empty, winSize);
        Swapchain = new Swapchain()
        {
            Size = winSize
        };

        _context.OMSetRenderTargets(_colorTargetView, _depthStencilTargetView);
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
            _context.RSSetViewport(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height);
        }
    }

    private Rectangle _scissor;

    public override Rectangle Scissor
    {
        get => _scissor;
        set
        {
            _scissor = value;
            _context.RSSetScissorRect(_scissor.X, _scissor.Y, _scissor.Width, _scissor.Height);
        }
    }

    public override void ClearColorBuffer(Color color) =>
        ClearColorBuffer(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

    public override void ClearColorBuffer(Vector4 color) => ClearColorBuffer(color.X, color.Y, color.Z, color.W);

    public override void ClearColorBuffer(float r, float g, float b, float a)
    {
        Color4 color = new Color4(r, g, b, a);
        
        if (_currentFramebuffer != null)
        {
            for (int i = 0; i < _currentFramebuffer.Targets.Length; i++)
                _context.ClearRenderTargetView(_currentFramebuffer.Targets[i], color);
        }
        else
            _context.ClearRenderTargetView(_colorTargetView, color);
    }

    public override void ClearDepthStencilBuffer(ClearFlags flags, float depth, byte stencil)
    {
        //_context.RSSetViewport(Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height);
        uint cf = 0;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            cf |= (uint) ClearFlag.Depth;

        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            cf |= (uint) ClearFlag.Stencil;

        _context.ClearDepthStencilView(_currentFramebuffer?.DepthStencil ?? _depthStencilTargetView, cf, depth, stencil);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        fixed (void* dat = data) 
            return new D3D11GraphicsBuffer(_device, _context, bufferType, (uint) (data.Length * sizeof(T)), dat, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(_device, _context, bufferType, (uint) sizeof(T), Unsafe.AsPointer(ref data), dynamic);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(_device, _context, bufferType, sizeInBytes, null, dynamic);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, IntPtr data, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(_device, _context, bufferType, sizeInBytes, data.ToPointer(), dynamic);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, void* data, bool dynamic = false)
    {
        return new D3D11GraphicsBuffer(_device, _context, bufferType, sizeInBytes, data, dynamic);
    }

    public override Texture CreateTexture(TextureDescription description)
    {
        return new D3D11Texture(_device, _context, description, null);
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[] data)
    {
        fixed (void* ptr = data)
            return new D3D11Texture(_device, _context, description, ptr);
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[][] data)
    {
        fixed (void* ptr = PieUtils.Combine(data))
            return new D3D11Texture(_device, _context, description, ptr);
    }

    public override Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        return new D3D11Texture(_device, _context, description, data.ToPointer());
    }

    public override Texture CreateTexture(TextureDescription description, void* data)
    {
        return new D3D11Texture(_device, _context, description, data);
    }

    public override Shader CreateShader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        return new D3D11Shader(_device, _context, attachments, constants);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new D3D11InputLayout(_device, descriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new D3D11RasterizerState(_device, description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new D3D11BlendState(_device, description);
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new D3D11DepthStencilState(_device, description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new D3D11SamplerState(_device, description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new D3D11Framebuffer(_device, attachments);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        fixed (void* dat = data)
            ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, (uint) (data.Length * sizeof(T)), dat);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, (uint) sizeof(T), Unsafe.AsPointer(ref data));
    }

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, sizeInBytes, data.ToPointer());
    }

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        ((D3D11GraphicsBuffer) buffer).Update(offsetInBytes, sizeInBytes, data);
    }

    public override void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z,
        int width, int height, int depth, T[] data)
    {
        fixed (void* dat = data)
            ((D3D11Texture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, dat);
    }

    public override void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width,
        int height, int depth, IntPtr data)
    {
        ((D3D11Texture) texture).Update(x, y, z, width, height, depth, mipLevel, arrayIndex, data.ToPointer());
    }

    public override void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z,
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
        _context.PSSetShaderResources(bindingSlot, 1, ref tex.View);
        _context.PSSetSamplers(bindingSlot, 1, ref ((D3D11SamplerState) state).State);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        _context.RSSetState(((D3D11RasterizerState) state).State);
    }

    public override void SetBlendState(BlendState state)
    {
        _context.OMSetBlendState(((D3D11BlendState) state).State, null, uint.MaxValue);
    }

    public override void SetDepthStencilState(DepthStencilState state, int stencilRef)
    {
        _context.OMSetDepthStencilState(((D3D11DepthStencilState) state).State, (uint) stencilRef);
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
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
        
        _context.IASetPrimitiveTopology(topology);
    }

    public override void SetInputLayout(InputLayout layout)
    {
        _context.IASetInputLayout(((D3D11InputLayout) layout).Layout);
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride)
    {
        _context.IASetVertexBuffers(slot, 1, ref ((D3D11GraphicsBuffer) buffer).Buffer, stride, 0);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        Silk.NET.DXGI.Format fmt = type switch
        {
            IndexType.UShort => Silk.NET.DXGI.Format.FormatR16Uint,
            IndexType.UInt => Silk.NET.DXGI.Format.FormatR32Uint,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        _context.IASetIndexBuffer(((D3D11GraphicsBuffer) buffer).Buffer, fmt, 0);
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        D3D11GraphicsBuffer buf = (D3D11GraphicsBuffer) buffer;
        _context.VSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
        _context.PSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
        _context.GSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
        _context.CSSetConstantBuffers(bindingSlot, 1, ref buf.Buffer);
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        _currentFramebuffer = (D3D11Framebuffer) framebuffer;
        if (framebuffer == null)
            _context.OMSetRenderTargets(1, ref _colorTargetView, _depthStencilTargetView);
        else
        {
            _context.OMSetRenderTargets((uint) _currentFramebuffer.Targets.Length, ref _currentFramebuffer.Targets[0],
                _currentFramebuffer.DepthStencil);
        }

    }

    public override void Draw(uint vertexCount)
    {
        _context.Draw(vertexCount, 0);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += vertexCount / 3;
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
        _context.Draw(vertexCount, (uint) startVertex);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += vertexCount/ 3;
    }

    public override void DrawIndexed(uint indexCount)
    {
        _context.DrawIndexed(indexCount, 0, 0);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount / 3;
    }

    public override void DrawIndexed(uint indexCount, int startIndex)
    {
        _context.DrawIndexed(indexCount, (uint) startIndex, 0);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount/ 3;
    }

    public override void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        _context.DrawIndexed(indexCount, (uint) startIndex, baseVertex);
        PieMetrics.DrawCalls++;
        PieMetrics.TriCount += indexCount/ 3;
    }

    public override void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        _context.DrawIndexedInstanced( indexCount, instanceCount, 0, 0, 0);
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
        _context.OMSetRenderTargets(1, ref _colorTargetView, _depthStencilTargetView);
        
        PieMetrics.DrawCalls = 0;
        PieMetrics.TriCount = 0;
    }

    public override void ResizeSwapchain(Size newSize)
    {
        _context.Flush();
        _context.OMSetRenderTargets(0, (ID3D11RenderTargetView*) null, (ID3D11DepthStencilView*) null);
        _colorTargetView.Dispose();
        _colorTexture.Dispose();

        if (_depthFormat != null)
        {
            _depthStencilTargetView.Dispose();
            _depthStencilTexture.Dispose();
        }

        if (!Succeeded(_swapChain.ResizeBuffers(0, (uint) newSize.Width, (uint) newSize.Height, Silk.NET.DXGI.Format.FormatUnknown, (uint) SwapChainFlag.AllowTearing | (uint) SwapChainFlag.AllowModeSwitch)))
            throw new PieException("Failed to resize swapchain buffers.");
        if (!Succeeded(_swapChain.GetBuffer(0, out _colorTexture)))
            throw new PieException("Failed to get swapchain buffer.");
        if (!Succeeded(_device.CreateRenderTargetView(_colorTexture, null, ref _colorTargetView)))
            throw new PieException("Failed to create swapchain color target.");
        CreateDepthStencilView(newSize);

        Swapchain.Size = newSize;
    }

    public override void GenerateMipmaps(Texture texture)
    {
        _context.GenerateMips(((D3D11Texture) texture).View);
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        _context.Dispatch(groupCountX, groupCountY, groupCountZ);
    }

    public override void Flush()
    {
        _context.Flush();
    }

    public override void Dispose()
    {
        _colorTargetView.Dispose();
        _swapChain.Dispose();
        _device.Dispose();
        _context.Dispose();
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

        if (!Succeeded(_device.CreateTexture2D(&texDesc, null, ref _depthStencilTexture)))
            throw new PieException("Failed to create swapchain depth texture.");

        if (!Succeeded(_device.CreateDepthStencilView(_depthStencilTexture, &viewDesc, ref _depthStencilTargetView)))
            throw new PieException("Failed to create swapchain depth view.");
    }
}