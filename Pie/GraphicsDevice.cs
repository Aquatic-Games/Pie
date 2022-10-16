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

    public abstract Rectangle Viewport { get; set; }
    
    public abstract Rectangle Scissor { get; set; }

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
    /// Create a texture with the given description and data.
    /// </summary>
    /// <param name="description">The description of the texture.</param>
    /// <param name="data">The initial data of the texture.</param>
    /// <typeparam name="T">Any unmanaged type (typically byte).</typeparam>
    /// <returns>The created texture.</returns>
    public abstract Texture CreateTexture<T>(TextureDescription description, T[] data = null) where T : unmanaged;

    /// <summary>
    /// Create a texture with the given description and data.
    /// </summary>
    /// <param name="description">The description of the texture.</param>
    /// <param name="data">The pointer to the initial data of the texture.</param>
    /// <returns>The created texture.</returns>
    public abstract Texture CreateTexture(TextureDescription description, IntPtr data);

    /// <summary>
    /// Create a shader with the given shader attachments.
    /// </summary>
    /// <param name="attachments">The attachments for this shader.</param>
    /// <remarks>This shader is not cross platform. Use Pie.ShaderCompiler's extension method to create a cross platform shader.</remarks>
    /// <returns>The created shader.</returns>
    public abstract Shader CreateShader(params ShaderAttachment[] attachments);

    /// <summary>
    /// Create an input layout which can be used with a vertex buffer, with a given stride value in bytes.
    /// </summary>
    /// <param name="descriptions">The descriptions for this layout.</param>
    /// <returns>The created input layout.</returns>
    public abstract InputLayout CreateInputLayout(params InputLayoutDescription[] inputLayoutDescriptions);

    /// <summary>
    /// Create a new rasterizer state with the default parameters, typical for 3D.
    /// </summary>
    /// <param name="face">The face that will be culled on next draw. Set to <see cref="CullFace.None"/> to disable face culling.</param>
    /// <param name="direction">The face cull direction. This will determine which is the front face and which is the back face.</param>
    /// <param name="fillMode">The fill mode that will be used on next draw.</param>
    /// <param name="enableScissor">Whether or not the scissor test will be enabled.</param>
    public abstract RasterizerState CreateRasterizerState(RasterizerStateDescription description);

    /// <summary>
    /// Create a new blend state from the blend state description.
    /// </summary>
    /// <param name="description">The blend state description to create from.</param>
    /// <returns>The created blend state.</returns>
    public abstract BlendState CreateBlendState(BlendStateDescription description);

    /// <summary>
    /// Create a new depth state from the depth state description.
    /// </summary>
    /// <param name="description">The depth state description to create from.</param>
    /// <returns>The created depth state.</returns>
    public abstract DepthState CreateDepthState(DepthStateDescription description);

    /// <summary>
    /// Create a new sampler state from the sampler state description.
    /// </summary>
    /// <param name="description">The sampler state description to create from.</param>
    /// <returns>The created sampler state.</returns>
    public abstract SamplerState CreateSamplerState(SamplerStateDescription description);

    /// <summary>
    /// Create a framebuffer, also known as a render target, that can be rendered to.
    /// </summary>
    /// <param name="attachments">The framebuffer attachments to attach.</param>
    /// <returns>The created framebuffer.</returns>
    public abstract Framebuffer CreateFramebuffer(params FramebufferAttachment[] attachments);
    
    /// <summary>
    /// Update the given buffer with the given data at the given offset in bytes.
    /// </summary>
    /// <param name="buffer">The buffer to update.</param>
    /// <param name="offsetInBytes">The offset in bytes, if any, where the data will be updated.</param>
    /// <param name="data">The data itself</param>
    /// <typeparam name="T">Any unmanaged type</typeparam>
    public abstract void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T[] data) where T : unmanaged;

    /// <summary>
    /// Update the given buffer with the given data at the given offset in bytes.
    /// </summary>
    /// <param name="buffer">The buffer to update.</param>
    /// <param name="offsetInBytes">The offset in bytes, if any, where the data will be updated.</param>
    /// <param name="data">The data itself</param>
    /// <typeparam name="T">Any unmanaged type</typeparam>
    public abstract void UpdateBuffer<T>(GraphicsBuffer buffer, uint offsetInBytes, T data) where T : unmanaged;
    
    /// <summary>
    /// Update a region of this texture with the given data.
    /// </summary>
    /// <param name="texture">The texture to update.</param>
    /// <param name="x">The x-offset in pixels of the data.</param>
    /// <param name="y">The y-offset in pixels of the data.</param>
    /// <param name="width">The width in pixels of the data.</param>
    /// <param name="height">The height in pixels of the data.</param>
    /// <param name="data">The data itself.</param>
    /// <typeparam name="T">Any unmanaged type, typically <see cref="byte"/> or <see cref="float"/>.</typeparam>
    public abstract void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data) where T : unmanaged;

    /// <summary>
    /// Set the shader that will be used on next draw.
    /// </summary>
    /// <param name="shader">The shader to use.</param>
    public abstract void SetShader(Shader shader);

    /// <summary>
    /// Set the texture that will be used on next draw.
    /// </summary>
    /// <param name="bindingSlot">The binding slot that this texture will be used in.</param>
    /// <param name="texture">The texture to use.</param>
    public abstract void SetTexture(uint bindingSlot, Texture texture, SamplerState samplerState);

    /// <summary>
    /// Set the rasterizer state that will be used on next draw.
    /// </summary>
    /// <param name="state">The rasterizer state to use.</param>
    public abstract void SetRasterizerState(RasterizerState state);

    /// <summary>
    /// Set the blend state that will be used on next draw.
    /// </summary>
    /// <param name="state">The blend state to use.</param>
    public abstract void SetBlendState(BlendState state);

    /// <summary>
    /// Set the depth state that will be used on next draw.
    /// </summary>
    /// <param name="state">The depth state to use.</param>
    public abstract void SetDepthState(DepthState state);

    /// <summary>
    /// Set the primitive type that will be used on next draw.
    /// </summary>
    /// <param name="type">The primitive type to draw with.</param>
    public abstract void SetPrimitiveType(PrimitiveType type);
    
    /// <summary>
    /// Set the vertex buffer that will be used on next draw.
    /// </summary>
    /// <param name="slot">The input slot.</param>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="stride">The stride, in bytes, for the input layout.</param>
    /// <param name="layout">The input layout that this vertex buffer will use.</param>
    public abstract void SetVertexBuffer(uint slot, GraphicsBuffer buffer, uint stride, InputLayout layout);

    /// <summary>
    /// Set the index buffer that will be used on next draw.
    /// </summary>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="type">The type of indices.</param>
    public abstract void SetIndexBuffer(GraphicsBuffer buffer, IndexType type);

    /// <summary>
    /// Set the uniform buffer that will be used on next draw.
    /// </summary>
    /// <param name="bindingSlot"></param>
    /// <param name="buffer">The buffer to use.</param>
    public abstract void SetUniformBuffer(uint bindingSlot, GraphicsBuffer buffer);

    /// <summary>
    /// Set the framebuffer that will be used on next draw. Set as null to use the default back buffer.
    /// </summary>
    /// <param name="framebuffer">The framebuffer to use.</param>
    public abstract void SetFramebuffer(Framebuffer framebuffer);

    /// <summary>
    /// Draw to the screen with the given number of vertices.
    /// </summary>
    /// <param name="vertexCount">The number of vertices.</param>
    public abstract void Draw(uint vertexCount);
    
    /// <summary>
    /// Draw to the screen with the given number of vertices, at the given start vertex.
    /// </summary>
    /// <param name="vertexCount">The number of vertices.</param>
    /// <param name="startVertex">The starting vertex of the vertices to draw.</param>
    public abstract void Draw(uint vertexCount, int startVertex);
    
    /// <summary>
    /// Draw to the screen with the given indices count.
    /// </summary>
    /// <param name="indexCount">The number of indices.</param>
    public abstract void DrawIndexed(uint indexCount);
    
    /// <summary>
    /// Draw to the screen with the given indices count, at the given start index.
    /// </summary>
    /// <param name="indexCount">The number of indices.</param>
    /// <param name="startIndex">The starting index of the indices to draw.</param>
    public abstract void DrawIndexed(uint indexCount, int startIndex);
    
    /// <summary>
    /// Draw to the screen with the given indices count, at the given start index, at the given base vertex.
    /// </summary>
    /// <param name="indexCount">The number of indices (indices).</param>
    /// <param name="startIndex">The starting index of the indices to draw.</param>
    /// <param name="baseVertex">The base vertex of the indices to draw.</param>
    public abstract void DrawIndexed(uint indexCount, int startIndex, int baseVertex);

    public abstract void DrawIndexedInstanced(uint indexCount, uint instanceCount);

    /// <summary>
    /// Present to the screen.
    /// </summary>
    public abstract void Present(int swapInterval);

    /// <summary>
    /// Resize the swapchain.
    /// </summary>
    /// <param name="newSize">The new size of the swapchain.</param>
    public abstract void ResizeSwapchain(Size newSize);

    /// <summary>
    /// Generate mipmaps for the given texture.
    /// </summary>
    /// <param name="texture"></param>
    public abstract void GenerateMipmaps(Texture texture);

    public abstract void Dispose();

    /// <summary>
    /// Create an OpenGL 3.3 graphics device.
    /// </summary>
    /// <param name="context">The GL context.</param>
    /// <param name="winSize">The size of the window on startup.</param>
    /// <param name="flags">The creation flags for this graphics device, if any.</param>
    /// <returns>The created graphics device.</returns>
    public static GraphicsDevice CreateOpenGL33(IGLContext context, Size winSize, GraphicsDeviceOptions options = default)
    {
        return new OpenGL33GraphicsDevice(context, winSize, options);
    }

    /// <summary>
    /// Create a Direct3D 11 graphics device.
    /// </summary>
    /// <param name="hwnd">The HWND pointer.</param>
    /// <param name="winSize">The size of the window on startup.</param>
    /// <param name="flags">The creation flags for this graphics device, if any.</param>
    /// <returns>The created graphics device.</returns>
    public static GraphicsDevice CreateD3D11(IntPtr hwnd, Size winSize, GraphicsDeviceOptions options = default)
    {
        return new D3D11GraphicsDevice(hwnd, winSize, options);
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