using System;
using System.Drawing;
using System.Numerics;
using Pie.OpenGL33;
using Silk.NET.Core.Contexts;

namespace Pie;

public abstract class GraphicsDevice : IDisposable
{
    public abstract void Clear(Color color, ClearFlags flags);

    public abstract void Clear(Vector4 color, ClearFlags flags);

    public abstract void Clear(ClearFlags flags);

    public abstract GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false);

    public abstract Texture CreateTexture(uint width, uint height, PixelFormat format);

    public abstract Shader CreateShader(params ShaderAttachment[] attachments);

    public abstract InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions);

    public abstract void SetShader(Shader shader);

    public abstract void SetTexture(uint slot, Texture texture);
    
    public abstract void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout);

    public abstract void SetIndexBuffer(GraphicsBuffer buffer);

    public abstract void Draw(uint elements);

    public abstract void Present();

    public abstract void ResizeMainFramebuffer(Size newSize);

    public abstract void Dispose();

    public static GraphicsDevice CreateOpenGL33(IGLContext context, bool debug)
    {
        return new OpenGL33GraphicsDevice(context, debug);
    }
}