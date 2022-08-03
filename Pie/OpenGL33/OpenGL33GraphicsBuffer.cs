using System;
using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33GraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly uint Handle;
    public readonly BufferTargetARB Target;

    public unsafe OpenGL33GraphicsBuffer(BufferType type, uint sizeInBytes, bool dynamic)
    {
        Target = type switch
        {
            BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        BufferUsageARB usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        Handle = Gl.GenBuffer();
        Gl.BindBuffer(Target, Handle);
        Gl.BufferData(Target, sizeInBytes, null, usage);
    }

    public override unsafe void Update<T>(uint offset, T[] data)
    {
        Gl.BindBuffer(Target, Handle);
        fixed (void* d = data)
            Gl.BufferSubData(Target, (nint) offset * Unsafe.SizeOf<T>(), (nuint) (data.Length * Unsafe.SizeOf<T>()), d);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Gl.DeleteBuffer(Handle);
    }
}