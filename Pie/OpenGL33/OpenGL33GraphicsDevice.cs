using System;
using System.Drawing;
using System.Numerics;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Pie.OpenGL33;

internal class OpenGL33GraphicsDevice : GraphicsDevice
{
    private IGLContext _context;
    internal static GL Gl;
    internal static bool Debug;
    
    // The poor, lone vao that powers the entire GL graphics device.
    private uint _vao;
    
    public OpenGL33GraphicsDevice(IGLContext context, bool debug)
    {
        _context = context;
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        Debug = debug;
    }
    
    public override void Clear(Color color, ClearFlags flags)
    {
        Vector4 nC = color.Normalize();
        Clear(nC, flags);
    }

    public override void Clear(Vector4 color, ClearFlags flags)
    {
        Gl.ClearColor(color.X, color.Y, color.Z, color.W);

        uint mask = 0;
        if ((flags & ClearFlags.Color) == ClearFlags.Color)
            mask |= (uint) ClearBufferMask.ColorBufferBit;
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            mask |= (uint) ClearBufferMask.DepthBufferBit;
        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            mask |= (uint) ClearBufferMask.StencilBufferBit;
        Gl.Clear(mask);
    }

    public override void Clear(ClearFlags flags)
    {
        uint mask = 0;
        if ((flags & ClearFlags.Color) == ClearFlags.Color)
            throw new PieException(
                "Cannot clear color bit without a color, use an overload with a color parameter instead.");
        if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            mask |= (uint) ClearBufferMask.DepthBufferBit;
        if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            mask |= (uint) ClearBufferMask.StencilBufferBit;
        Gl.Clear(mask);
    }

    public override GraphicsBuffer CreateBuffer(BufferType bufferType, uint sizeInBytes, bool dynamic = false)
    {
        return new OpenGL33GraphicsBuffer(bufferType, sizeInBytes, dynamic);
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new OpenGL33Shader(attachments);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new OpenGL33InputLayout(descriptions);
    }

    public override void SetShader(Shader shader)
    {
        OpenGL33Shader glShader = (OpenGL33Shader) shader;
        Gl.UseProgram(glShader.Handle);
        OpenGL33Shader.BoundHandle = glShader.Handle;
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        ((OpenGL33InputLayout) layout).Set(OpenGL33Shader.BoundHandle);
        OpenGL33GraphicsBuffer glBuf = (OpenGL33GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ArrayBuffer)
            throw new PieException("Given buffer is not a vertex buffer.");
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, glBuf.Handle);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer)
    {
        OpenGL33GraphicsBuffer glBuf = (OpenGL33GraphicsBuffer) buffer;
        if (glBuf.Target != BufferTargetARB.ElementArrayBuffer)
            throw new PieException("Given buffer is not an index buffer.");
        Gl.BindBuffer(GLEnum.ElementArrayBuffer, glBuf.Handle);
    }

    public override unsafe void Draw(uint elements)
    {
        Gl.DrawElements(PrimitiveType.Triangles, elements, DrawElementsType.UnsignedInt, null);
    }

    public override void Present()
    {
        _context.SwapBuffers();
    }

    public override void ResizeMainFramebuffer(Size newSize)
    {
        Gl.Viewport(0, 0, (uint) newSize.Width, (uint) newSize.Height);
    }

    public override void Dispose()
    {
        Gl.BindVertexArray(0);
        Gl.DeleteVertexArray(_vao);
    }
}