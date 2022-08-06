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

    public OpenGL33GraphicsBuffer(uint handle, BufferTargetARB target)
    {
        Handle = handle;
        Target = target;
    }

    public static unsafe GraphicsBuffer CreateBuffer<T>(BufferType type, uint sizeInBytes, T[] data, bool dynamic) where T : unmanaged
    {
        BufferTargetARB target = type switch
        {
            BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            BufferType.UniformBuffer => BufferTargetARB.UniformBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        BufferUsageARB usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        uint handle = Gl.GenBuffer();
        Gl.BindBuffer(target, handle);
        if (data == null)
            Gl.BufferData(target, sizeInBytes, null, usage);
        else
        {
            fixed (void* d = data)
                Gl.BufferData(target, sizeInBytes, d, usage);
        }

        switch (type)
        {
            case BufferType.VertexBuffer:
                PieMetrics.VertexBufferCount++;
                break;
            case BufferType.IndexBuffer:
                PieMetrics.IndexBufferCount++;
                break;
            case BufferType.UniformBuffer:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return new OpenGL33GraphicsBuffer(handle, target);
    }

    public override unsafe void Update<T>(uint offsetInBytes, T[] data)
    {
        Gl.BindBuffer(Target, Handle);
        fixed (void* d = data)
            Gl.BufferSubData(Target, (nint) offsetInBytes * Unsafe.SizeOf<T>(), (nuint) (data.Length * Unsafe.SizeOf<T>()), d);
    }

    public override unsafe void Update<T>(uint offsetInBytes, T data)
    {
        Gl.BindBuffer(Target, Handle);
        Gl.BufferSubData(Target, (nint) offsetInBytes * Unsafe.SizeOf<T>(), (nuint) Unsafe.SizeOf<T>(), data);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Gl.DeleteBuffer(Handle);
        switch (Target)
        {
            case BufferTargetARB.ArrayBuffer:
                PieMetrics.VertexBufferCount--;
                break;
            case BufferTargetARB.ElementArrayBuffer:
                PieMetrics.IndexBufferCount--;
                break;
        }
    }
}