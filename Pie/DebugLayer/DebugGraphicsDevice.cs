using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Pie.DebugLayer;
using Pie.ShaderCompiler;

namespace Pie.Debugging;

internal sealed unsafe class DebugGraphicsDevice : GraphicsDevice
{
    private GraphicsDevice _device;

    public DebugGraphicsDevice(GraphicsDevice device)
    {
        _device = device;
        
        PieLog.Log(LogType.Info, "Debug graphics device initialized.");
        PieLog.Log(LogType.Info, $"Adapter: {Adapter.Name}");
        PieLog.Log(LogType.Info, $"Backend: {Api}");
    }

    public override GraphicsApi Api => _device.Api;

    public override Swapchain Swapchain => _device.Swapchain;

    public override GraphicsAdapter Adapter => _device.Adapter;

    public override Rectangle Viewport
    {
        get => _device.Viewport;
        set => _device.Viewport = value;
    }

    public override Rectangle Scissor
    {
        get => _device.Scissor;
        set => _device.Scissor = value;
    }
    
    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
        _device.Clear(color, flags);
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
       _device.Clear(color, flags);
    }

    public override void Clear(ClearFlags flags)
    {
        _device.Clear(flags);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        fixed (void* ptr = data)
            return new DebugGraphicsBuffer(_device, bufferType, (uint) (data.Length * sizeof(T)), ptr, dynamic);
    }

    public override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        return new DebugGraphicsBuffer(_device, bufferType, (uint) sizeof(T), Unsafe.AsPointer(ref data), dynamic);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        return new DebugGraphicsBuffer(_device, bufferType, sizeInBytes, null, dynamic);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, IntPtr data, bool dynamic = false)
    {
        return new DebugGraphicsBuffer(_device, bufferType, sizeInBytes, (void*) data, dynamic);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, void* data, bool dynamic = false)
    {
        return new DebugGraphicsBuffer(_device, bufferType, sizeInBytes, data, dynamic);
    }

    public override Texture CreateTexture(TextureDescription description)
    {
        return new DebugTexture(_device, description, null);
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[] data)
    {
        fixed (void* ptr = data)
            return new DebugTexture(_device, description, ptr);
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[][] data)
    {
        fixed (void* ptr = PieUtils.Combine(data))
            return new DebugTexture(_device, description, ptr);
    }

    public override Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        return new DebugTexture(_device, description, (void*) data);
    }

    public override unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        return new DebugTexture(_device, description, data);
    }

    public override Shader CreateShader(ShaderAttachment[] attachments, SpecializationConstant[] constants = null)
    {
        return new DebugShader(_device, attachments, constants);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] inputLayoutDescriptions)
    {
        return new DebugInputLayout(_device, inputLayoutDescriptions);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new DebugRasterizerState(_device, description);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        throw new NotImplementedException();
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new DebugDepthStencilState(_device, description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        throw new NotImplementedException();
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        throw new NotImplementedException();
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height,
        int depth, T[] data)
    {
        throw new NotImplementedException();
    }

    public override void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth,
        IntPtr data)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height,
        int depth, void* data)
    {
        throw new NotImplementedException();
    }

    public override IntPtr MapBuffer(GraphicsBuffer buffer, MapMode mode)
    {
        throw new NotImplementedException();
    }

    public override void UnmapBuffer(GraphicsBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public override void SetShader(Shader shader)
    {
        _device.SetShader(((DebugShader) shader).Shader);
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState samplerState)
    {
        throw new NotImplementedException();
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        _device.SetRasterizerState(((DebugRasterizerState) state).RasterizerState);
    }

    public override void SetBlendState(BlendState state)
    {
        throw new NotImplementedException();
    }

    public override void SetDepthStencilState(DepthStencilState state, int stencilRef = 0)
    {
        _device.SetDepthStencilState(((DebugDepthStencilState) state).DepthStencilState, stencilRef);
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        _device.SetPrimitiveType(type);
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        if (dBuffer.BufferType != BufferType.VertexBuffer)
            PieLog.Log(LogType.Critical, $"Expected VertexBuffer, buffer is an {dBuffer.BufferType} instead.");

        DebugInputLayout dLayout = (DebugInputLayout) layout;
        if (!dLayout.HasProducedStrideWarning && stride != dLayout.CalculatedStride)
        {
            dLayout.HasProducedStrideWarning = true;
            PieLog.Log(LogType.Warning, $"Potential invalid usage: Input layout stride was {stride}, but a stride of {dLayout.CalculatedStride} was expected.");
        }

        _device.SetVertexBuffer(slot, dBuffer.Buffer, stride, dLayout.InputLayout);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        if (dBuffer.BufferType != BufferType.IndexBuffer)
            PieLog.Log(LogType.Critical, $"Expected IndexBuffer, buffer is an {dBuffer.BufferType} instead.");
        
        _device.SetIndexBuffer(dBuffer.Buffer, type);
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        if (dBuffer.BufferType != BufferType.UniformBuffer)
            PieLog.Log(LogType.Critical, $"Expected UniformBuffer, buffer is an {dBuffer.BufferType} instead.");
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint vertexCount)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexed(uint indexCount)
    {
        _device.DrawIndexed(indexCount);
    }

    public override void DrawIndexed(uint indexCount, int startIndex)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        throw new NotImplementedException();
    }

    public override void Present(int swapInterval)
    {
        if (swapInterval > 4)
            PieLog.Log(LogType.Critical, $"Swap interval should be a maximum of 4, however an interval of {swapInterval} was provided.");
        
        _device.Present(swapInterval);
    }

    public override void ResizeSwapchain(Size newSize)
    {
        _device.ResizeSwapchain(newSize);
    }

    public override void GenerateMipmaps(Texture texture)
    {
        throw new NotImplementedException();
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        throw new NotImplementedException();
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        _device.Dispose();
    }
}