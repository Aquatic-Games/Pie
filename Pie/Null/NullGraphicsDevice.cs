using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pie.Null;

internal sealed class NullGraphicsDevice : GraphicsDevice
{
    public override GraphicsApi Api => GraphicsApi.Null;

    public override Swapchain Swapchain { get; }

    public override GraphicsAdapter Adapter { get;  }

    public override Rectangle Viewport { get; set; }
    public override Rectangle Scissor { get; set; }

    public NullGraphicsDevice()
    {
        Swapchain = new Swapchain();
        Adapter = new GraphicsAdapter("Null");
    }

    public override void Clear(Color color, ClearFlags flags = ClearFlags.None)
    {
    }

    public override void Clear(Vector4 color, ClearFlags flags = ClearFlags.None)
    {
    }

    public override void Clear(ClearFlags flags)
    {
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new NullBlendState();
    }

    public unsafe override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false)
    {
        var buffer = Marshal.AllocHGlobal(Unsafe.SizeOf<T>() * data.Length);
        Unsafe.Copy((void*)buffer, ref data[0]);
        return new NullGraphicsBuffer(buffer, true);
    }

    public unsafe override GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false)
    {
        var buffer = Marshal.AllocHGlobal(Unsafe.SizeOf<T>());
        Unsafe.Copy((void*)buffer, ref data);
        return new NullGraphicsBuffer(buffer, true);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        return new NullGraphicsBuffer(Marshal.AllocHGlobal((int)sizeInBytes), true);
    }

    public unsafe override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, IntPtr data, bool dynamic = false)
    {
        return new NullGraphicsBuffer(data, false);
    }

    public override unsafe GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, void* data, bool dynamic = false)
    {
        return new NullGraphicsBuffer((IntPtr)data, false);
    }

    public override DepthState CreateDepthState(DepthStateDescription description)
    {
        return new NullDepthState();
    }

    public override Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments)
    {
        return new NullFramebuffer();
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] inputLayoutDescriptions)
    {
        return new NullInputLayout();
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new NullRasterizerState();
    }

    public override SamplerState CreateSamplerState(SamplerStateDescription description)
    {
        return new NullSamplerState();
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new NullShader();
    }

    public override Texture CreateTexture(TextureDescription description)
    {
        return new NullTexture();
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[] data)
    {
        return new NullTexture();
    }

    public override Texture CreateTexture<T>(TextureDescription description, T[][] data)
    {
        return new NullTexture();
    }

    public override Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        return new NullTexture();
    }

    public override unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        return new NullTexture();
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
    }

    public override void Dispose()
    {
    }

    public override void Draw(uint vertexCount)
    {
    }

    public override void Draw(uint vertexCount, int startVertex)
    {
    }

    public override void DrawIndexed(uint indexCount)
    {
    }

    public override void DrawIndexed(uint indexCount, int startIndex)
    {
    }

    public override void DrawIndexed(uint indexCount, int startIndex, int baseVertex)
    {
    }

    public override void DrawIndexedInstanced(uint indexCount, uint instanceCount)
    {
    }

    public override void Flush()
    {
    }

    public override void GenerateMipmaps(Texture texture)
    {
    }

    public override IntPtr MapBuffer(GraphicsBuffer buffer, MapMode mode)
    {
        return ((NullGraphicsBuffer)buffer).Data;
    }

    public override void Present(int swapInterval)
    {
    }

    public override void ResizeSwapchain(Size newSize)
    {
    }

    public override void SetBlendState(BlendState state)
    {
    }

    public override void SetDepthState(DepthState state)
    {
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
    }

    public override void SetPrimitiveType(PrimitiveType type)
    {
    }

    public override void SetRasterizerState(RasterizerState state)
    {
    }

    public override void SetShader(Shader shader)
    {
    }

    public override void SetTexture(uint bindingSlot, Texture texture, SamplerState samplerState)
    {
    }

    public override void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer)
    {
    }

    public override void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout)
    {
    }

    public override void UnmapBuffer(GraphicsBuffer buffer)
    {
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data)
    {
    }

    public override void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data)
    {
    }

    public override void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, IntPtr data)
    {
    }

    public override unsafe void UpdateBuffer(GraphicsBuffer buffer, uint offsetInBytes, uint sizeInBytes, void* data)
    {
    }

    public override void UpdateTexture<T>(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth, T[] data)
    {
    }

    public override void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth, IntPtr data)
    {
    }

    public override unsafe void UpdateTexture(Texture texture, int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth, void* data)
    {
    }
}
