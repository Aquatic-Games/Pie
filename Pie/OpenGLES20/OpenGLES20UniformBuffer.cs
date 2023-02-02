using System;

namespace Pie.OpenGLES20;

internal sealed class OpenGLES20UniformBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public static OpenGLES20UniformBuffer CreateBuffer<T>(T[] data) where T : unmanaged
    {
        Console.WriteLine(data.GetType().GetElementType());

        return null;
    }
    
    public override void Dispose()
    {
        if (!IsDisposed)
            IsDisposed = true;
    }
}