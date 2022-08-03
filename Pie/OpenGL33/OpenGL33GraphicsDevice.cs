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
    
    // The poor, lone vao that powers the entire GL graphics device.
    private uint _vao;
    
    public OpenGL33GraphicsDevice(IGLContext context)
    {
        _context = context;
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
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

    public override void Present()
    {
        _context.SwapBuffers();
    }

    public override void Dispose()
    {
        Gl.BindVertexArray(0);
        Gl.DeleteVertexArray(_vao);
    }
}