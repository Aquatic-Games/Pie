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

    private bool _vertexBufferSet;
    private bool _indexBufferSet;
    private bool _shaderSet;

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
        return new DebugBlendState(_device, description);
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new DebugDepthStencilState(_device, description);
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new DebugSamplerState(_device, description);
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new DebugFramebuffer(_device, attachments);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
        fixed (void* ptr = data)
            ((DebugGraphicsBuffer) buffer).Update(_device, offsetInBytes, (uint) (data.Length * sizeof(T)), ptr);
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
        ((DebugGraphicsBuffer) buffer).Update(_device, offsetInBytes, (uint) sizeof(T), Unsafe.AsPointer(ref data));
    }

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
        ((DebugGraphicsBuffer) buffer).Update(_device, offsetInBytes, sizeInBytes, (void*) data);
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
        ((DebugGraphicsBuffer) buffer).Update(_device, offsetInBytes, sizeInBytes, data);
    }

    public override void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height,
        int depth, T[] data)
    {
        fixed (void* ptr = data)
            ((DebugTexture) texture).Update(_device, mipLevel, arrayIndex, x, y, z, width, height, depth, ptr);
    }

    public override void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth,
        IntPtr data)
    {
        ((DebugTexture) texture).Update(_device, mipLevel, arrayIndex, x, y, z, width, height, depth, (void*) data);
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height,
        int depth, void* data)
    {
        ((DebugTexture) texture).Update(_device, mipLevel, arrayIndex, x, y, z, width, height, depth, data);
    }

    public override IntPtr MapBuffer(GraphicsBuffer buffer, MapMode mode)
    {
        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        
        if (dBuffer.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to map a disposed buffer!");
        
        if (!dBuffer.IsDynamic)
            PieLog.Log(LogType.Critical, "Cannot map a dynamic buffer.");

        if (dBuffer.IsMapped)
            PieLog.Log(LogType.Critical, "Cannot map a buffer that has already been mapped.");

        dBuffer.IsMapped = true;

        return _device.MapBuffer(dBuffer.Buffer, mode);
    }

    public override void UnmapBuffer(GraphicsBuffer buffer)
    {
        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        
        if (dBuffer.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to unmap a disposed buffer!");
        
        if (!dBuffer.IsMapped)
            PieLog.Log(LogType.Critical, "Cannot unmap a buffer that has not been mapped.");
        
        // This should never happen but it doesn't hurt to have the check!
        if (!dBuffer.IsDynamic)
            PieLog.Log(LogType.Critical, "Attempted to unmap a dynamic buffer. If you see this message, Pie's checks have gone wrong. Either that or something MAJORLY bad has happened. Panic.");

        dBuffer.IsMapped = false;
        
        _device.UnmapBuffer(dBuffer.Buffer);
    }

    public override void SetShader(Shader shader)
    {
        if (shader.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed shader!");

        _shaderSet = true;
        _device.SetShader(((DebugShader) shader).Shader);
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState samplerState)
    {
        if (texture.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed texture!");

        _device.SetTexture(bindingSlot, ((DebugTexture) texture).Texture, ((DebugSamplerState) samplerState).SamplerState);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        if (state.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed rasterizer state!");

        _device.SetRasterizerState(((DebugRasterizerState) state).RasterizerState);
    }

    public override void SetBlendState(BlendState state)
    {
        if (state.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed blend state!");

        _device.SetBlendState(((DebugBlendState) state).BlendState);
    }

    public override void SetDepthStencilState(DepthStencilState state, int stencilRef = 0)
    {
        if (state.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed depth stencil state!");

        _device.SetDepthStencilState(((DebugDepthStencilState) state).DepthStencilState, stencilRef);
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
        _device.SetPrimitiveType(type);
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
        if (buffer.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed buffer!");

        if (layout.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed input layout!");

        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        if (dBuffer.BufferType != BufferType.VertexBuffer)
            PieLog.Log(LogType.Critical, $"Expected VertexBuffer, buffer is an {dBuffer.BufferType} instead.");

        DebugInputLayout dLayout = (DebugInputLayout) layout;
        if (!dLayout.HasProducedStrideWarning && stride != dLayout.CalculatedStride)
        {
            dLayout.HasProducedStrideWarning = true;
            PieLog.Log(LogType.Warning, $"Potential invalid usage: Input layout stride was {stride}, but a stride of {dLayout.CalculatedStride} was expected.");
        }

        _vertexBufferSet = true;
        _device.SetVertexBuffer(slot, dBuffer.Buffer, stride, dLayout.InputLayout);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        if (buffer.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed buffer!");

        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        if (dBuffer.BufferType != BufferType.IndexBuffer)
            PieLog.Log(LogType.Critical, $"Expected IndexBuffer, buffer is an {dBuffer.BufferType} instead.");

        _indexBufferSet = true;
        _device.SetIndexBuffer(dBuffer.Buffer, type);
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
        if (buffer.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed buffer!");

        DebugGraphicsBuffer dBuffer = (DebugGraphicsBuffer) buffer;
        if (dBuffer.BufferType != BufferType.UniformBuffer)
            PieLog.Log(LogType.Critical, $"Expected UniformBuffer, buffer is an {dBuffer.BufferType} instead.");
        
        _device.SetUniformBuffer(bindingSlot, dBuffer.Buffer);
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        if (framebuffer == null)
        {
            _device.SetFramebuffer(null);
            return;
        }

        DebugFramebuffer dFb = (DebugFramebuffer) framebuffer;

        if (dFb.IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to set a disposed framebuffer!");
        
        foreach (FramebufferAttachment attachment in dFb.Attachments)
        {
            if (attachment.Texture.IsDisposed)
                PieLog.Log(LogType.Critical, "Attached framebuffer texture has been disposed since the framebuffer was created.");
        }
        
        _device.SetFramebuffer(dFb.Framebuffer);
    }

    public override void Draw(uint vertexCount)
    {
        if (!_shaderSet)
            PieLog.Log(LogType.Critical, "Attempted to draw, however no shader has been set.");
        
        if (!_vertexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw, however no vertex buffer has been set.");
        
        _device.Draw(vertexCount);
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
        if (!_shaderSet)
            PieLog.Log(LogType.Critical, "Attempted to draw, however no shader has been set.");
        
        if (!_vertexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw, however no vertex buffer has been set.");
        
        //if (startVertex >= vertexCount)
        //    PieLog.Log(LogType.Critical, $"The vertex count was {vertexCount}, but the start vertex was {startVertex}.");
        
        _device.Draw(vertexCount, startVertex);
    }

    public override void DrawIndexed(uint indexCount)
    {
        if (!_shaderSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no shader has been set.");
        
        if (!_vertexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no vertex buffer has been set.");
        
        if (!_indexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no index buffer has been set.");
        
        _device.DrawIndexed(indexCount);
    }

    public override void DrawIndexed(uint indexCount, int startIndex)
    {
        if (!_shaderSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no shader has been set.");
        
        if (!_vertexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no vertex buffer has been set.");
        
        if (!_indexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no index buffer has been set.");
        
        //if (startIndex >= indexCount)
        //    PieLog.Log(LogType.Critical, $"The index count was {indexCount}, but the start index was {startIndex}.");
        
        _device.DrawIndexed(indexCount, startIndex);
    }

    public override void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
        if (!_shaderSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no shader has been set.");
        
        if (!_vertexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no vertex buffer has been set.");
        
        if (!_indexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed, however no index buffer has been set.");
        
        //if (startIndex >= indexCount)
        //    PieLog.Log(LogType.Critical, $"The index count was {indexCount}, but the start index was {startIndex}.");
        
        _device.DrawIndexed(indexCount, startIndex, baseVertex);
    }

    public override void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
        if (!_shaderSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed instanced, however no shader has been set.");
        
        if (!_vertexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed instanced, however no vertex buffer has been set.");
        
        if (!_indexBufferSet)
            PieLog.Log(LogType.Critical, "Attempted to draw indexed instanced, however no index buffer has been set.");
        
        _device.DrawIndexedInstanced(indexCount, instanceCount);
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
        DebugTexture dTexture = (DebugTexture) texture;
        if (dTexture.Description.MipLevels == 1)
            PieLog.Log(LogType.Warning, "Attempting to generate mipmaps for a texture that does not support mipmaps.");

        _device.GenerateMipmaps(dTexture.Texture);
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        _device.Dispatch(groupCountX, groupCountY, groupCountZ);
    }

    public override void Flush()
    {
        _device.Flush();
    }

    public override void Dispose()
    {
        _device.Dispose();
        PieLog.Log(LogType.Info, DebugMetrics.GetString());
    }
}