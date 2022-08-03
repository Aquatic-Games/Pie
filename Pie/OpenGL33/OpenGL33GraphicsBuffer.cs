using System;
using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33GraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public uint Handle;
    private BufferTargetARB _target;

    public unsafe OpenGL33GraphicsBuffer(BufferType type, uint sizeInBytes, bool dynamic)
    {
        _target = type switch
        {
            BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        BufferUsageARB usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        Handle = Gl.GenBuffer();
        Gl.BindBuffer(_target, Handle);
        Gl.BufferData(_target, sizeInBytes, null, usage);
    }

    public override unsafe void Update<T>(uint offset, T[] data)
    {
        Gl.BindBuffer(_target, Handle);
        fixed (void* d = data)
            Gl.BufferSubData(_target, (nint) offset * Unsafe.SizeOf<T>(), (nuint) (data.Length * Unsafe.SizeOf<T>()), d);
    }

    public override void Dispose()
    {
        Gl.DeleteBuffer(Handle);
    }
}