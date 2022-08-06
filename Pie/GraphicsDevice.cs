using System;
using System.Drawing;
using System.Numerics;
using Pie.Direct3D11;
using Pie.OpenGL33;
using Silk.NET.Core.Contexts;

namespace Pie;

public abstract class GraphicsDevice : IDisposable
{
    public abstract GraphicsApi Api { get; }
    
    public abstract RasterizerState RasterizerState { get; set; }
    
    public abstract DepthMode DepthMode { get; set; }
    
    public abstract Rectangle Viewport { get; set; }
    
    public abstract bool VSync { get; set; }
    
    /// <summary>
    /// Clears the set Framebuffer with the given color and flags. If no framebuffer is set, this clears the back buffer.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    /// <param name="flags">The flags for clearing other bits.</param>
    public abstract void Clear(Color color, ClearFlags flags = ClearFlags.None);

    /// <summary>
    /// Clears the set Framebuffer with the given normalized color and flags. If no framebuffer is set, this clears the
    /// back buffer.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    /// <param name="flags">The flags for clearing other bits.</param>
    public abstract void Clear(Vector4 color, ClearFlags flags = ClearFlags.None);

    /// <summary>
    /// Clears the set Framebuffer with the given flags. If no framebuffer is set, this clears the back buffer.
    /// </summary>
    /// <param name="flags">The flags for clearing bits.</param>
    public abstract void Clear(ClearFlags flags);

    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false) where T : unmanaged;
    
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false) where T : unmanaged;
    
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T[] data, bool dynamic = false) where T : unmanaged;
    
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T data, bool dynamic = false) where T : unmanaged;

    public abstract Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear, bool mipmap = true);

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

    public static GraphicsDevice CreateOpenGL33(IGLContext context, Size winSize, bool vsync, bool debug)
    {
        return new OpenGL33GraphicsDevice(context, winSize, vsync, debug);
    }

    public static GraphicsDevice CreateD3D11(IntPtr hwnd, Size winSize, bool vsync, bool debug)
    {
        return new D3D11GraphicsDevice(hwnd, winSize, vsync, debug);
    }
}