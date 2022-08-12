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

    /// <summary>
    /// Create a graphics buffer with the given type and data.
    /// </summary>
    /// <param name="bufferType">The type of buffer that should be created.</param>
    /// <param name="data">The data itself.</param>
    /// <param name="dynamic">Whether or not this buffer is dynamic. You <b>MUST</b> set this value if you plan on updating this buffer's data afterwards.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <remarks>The sizeInBytes as found in other overloads is calculated as <c>data.Length * sizeof(T)</c></remarks>
    /// <returns>The created graphics buffer.</returns>
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T[] data, bool dynamic = false) where T : unmanaged;
    
    /// <summary>
    /// Create a graphics buffer with the given type and data.
    /// </summary>
    /// <param name="bufferType">The type of buffer that should be created.</param>
    /// <param name="data">The data itself.</param>
    /// <param name="dynamic">Whether or not this buffer is dynamic. You <b>MUST</b> set this value if you plan on updating this buffer's data afterwards.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <remarks>The sizeInBytes as found in other overloads is calculated as <c>sizeof(T)</c></remarks>
    /// <returns>The created graphics buffer.</returns>
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, T data, bool dynamic = false) where T : unmanaged;
    
    /// <summary>
    /// Create a graphics buffer with the given type and data.
    /// </summary>
    /// <param name="bufferType">The type of buffer that should be created.</param>
    /// <param name="sizeInBytes">The size in bytes of the buffer.</param>
    /// <param name="data">The data itself.</param>
    /// <param name="dynamic">Whether or not this buffer is dynamic. You <b>MUST</b> set this value if you plan on updating this buffer's data afterwards.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <returns>The created graphics buffer.</returns>
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T[] data, bool dynamic = false) where T : unmanaged;
    
    /// <summary>
    /// Create a graphics buffer with the given type and data.
    /// </summary>
    /// <param name="bufferType">The type of buffer that should be created.</param>
    /// <param name="sizeInBytes">The size in bytes of the buffer.</param>
    /// <param name="data">The data itself.</param>
    /// <param name="dynamic">Whether or not this buffer is dynamic. You <b>MUST</b> set this value if you plan on updating this buffer's data afterwards.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <returns>The created graphics buffer.</returns>
    public abstract GraphicsBuffer CreateBuffer<T>(BufferType bufferType, uint sizeInBytes, T data, bool dynamic = false) where T : unmanaged;

    /// <summary>
    /// Create a texture with the given data.
    /// </summary>
    /// <param name="width">The width, in pixels, of this texture.</param>
    /// <param name="height">The height, in pixels, of this texture.</param>
    /// <param name="format">The pixel format of the input data.</param>
    /// <param name="data">The data itself.</param>
    /// <param name="sample">The sample type of the texture.</param>
    /// <param name="mipmap">If true, mipmaps will be automatically generated on creation, as well as whenever <see cref="Texture.Update(int,int,uint,uint,x[])"/> is called.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    /// <returns>The created texture.</returns>
    public abstract Texture CreateTexture<T>(uint width, uint height, PixelFormat format, T[] data, TextureSample sample = TextureSample.Linear, bool mipmap = true) where T : unmanaged;

    /// <summary>
    /// Create a shader with the given shader attachments.
    /// </summary>
    /// <param name="attachments">The attachments for this shader.</param>
    /// <remarks>This shader is not cross platform. Use Pie.ShaderCompiler's extension method to create a cross platform shader.</remarks>
    /// <returns>The created shader.</returns>
    public abstract Shader CreateShader(params ShaderAttachment[] attachments);

    /// <summary>
    /// Create an input layout which can be used with a vertex buffer.
    /// </summary>
    /// <param name="descriptions">The descriptions for this layout.</param>
    /// <returns>The created input layout.</returns>
    public abstract InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions);

    /// <summary>
    /// Set the shader that will be used on next draw.
    /// </summary>
    /// <param name="shader">The shader to use.</param>
    public abstract void SetShader(Shader shader);

    /// <summary>
    /// Set the texture that will be used on next draw.
    /// </summary>
    /// <param name="slot">The slot (texture unit) to set this shader.</param>
    /// <param name="texture">The texture to use.</param>
    public abstract void SetTexture(uint slot, Texture texture);
    
    /// <summary>
    /// Set the vertex buffer that will be used on next draw.
    /// </summary>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="layout">The input layout to use for this vertex buffer.</param>
    public abstract void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout);

    /// <summary>
    /// Set the index buffer that will be used on next draw.
    /// </summary>
    /// <param name="buffer">The buffer to use.</param>
    public abstract void SetIndexBuffer(GraphicsBuffer buffer);

    /// <summary>
    /// Set the uniform buffer that will be used on next draw.
    /// </summary>
    /// <param name="stage">The stage that this uniform buffer is contained in.</param>
    /// <param name="slot">The binding slot that this uniform buffer is.</param>
    /// <param name="buffer">The buffer to use.</param>
    public abstract void SetUniformBuffer(ShaderStage stage, uint slot, GraphicsBuffer buffer);

    /// <summary>
    /// Draw to the screen with the given elements count.
    /// </summary>
    /// <param name="elements">The number of elements (indices).</param>
    public abstract void Draw(uint elements);

    /// <summary>
    /// Present to the screen.
    /// </summary>
    public abstract void Present();

    /// <summary>
    /// Resize the main framebuffer.
    /// </summary>
    /// <param name="newSize">The new size of the framebuffer.</param>
    public abstract void ResizeMainFramebuffer(Size newSize);

    public abstract void Dispose();

    /// <summary>
    /// Create an OpenGL 3.3 graphics device.
    /// </summary>
    /// <param name="context">The GL context.</param>
    /// <param name="winSize">The size of the window on startup.</param>
    /// <param name="vsync">Whether vsync is enabled.</param>
    /// <param name="debug">Whether debug is enabled.</param>
    /// <returns></returns>
    public static GraphicsDevice CreateOpenGL33(IGLContext context, Size winSize, bool vsync, bool debug)
    {
        return new OpenGL33GraphicsDevice(context, winSize, vsync, debug);
    }

    /// <summary>
    /// Create a Direct3D 11 graphics device.
    /// </summary>
    /// <param name="hwnd">The HWND pointer.</param>
    /// <param name="winSize">The size of the window on startup.</param>
    /// <param name="vsync">Whether vsync is enabled.</param>
    /// <param name="debug">Whether debug is enabled.</param>
    /// <returns></returns>
    public static GraphicsDevice CreateD3D11(IntPtr hwnd, Size winSize, bool vsync, bool debug)
    {
        return new D3D11GraphicsDevice(hwnd, winSize, vsync, debug);
    }

    /// <summary>
    /// Determine the best graphics API to use for the current platform.
    /// </summary>
    /// <returns></returns>
    public static GraphicsApi GetBestApiForPlatform()
    {
        if (OperatingSystem.IsWindows())
            return GraphicsApi.D3D11;
        else
            return GraphicsApi.OpenGl33;
    }
}