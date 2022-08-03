using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33GraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public uint Handle;
    private BufferTargetARB _target;
    private BufferUsageARB _usage;

    public unsafe OpenGL33GraphicsBuffer(BufferType type, uint sizeInBytes, bool dynamic)
    {
        _target = type switch
        {
            BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        _usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        Handle = Gl.GenBuffer();
        Gl.BindBuffer(_target, Handle);
        Gl.BufferData(_target, sizeInBytes, null, _usage);
    }

    public override void Update<T>(uint offset, T[] data)
    {
        
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}