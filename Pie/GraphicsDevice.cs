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

    public abstract void Present();
    
    public abstract void Dispose();

    public static GraphicsDevice CreateOpenGL33(IGLContext context)
    {
        return new OpenGL33GraphicsDevice(context);
    }
}