using System;
using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33GraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly uint Handle;
    public readonly BufferTargetARB Target;

    public OpenGL33GraphicsBuffer(uint handle, BufferTargetARB target)
    {
        Handle = handle;
        Target = target;
    }

    public static unsafe GraphicsBuffer CreateBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        BufferTargetARB target;

        switch (type)
        {
            case BufferType.VertexBuffer:
                target = BufferTargetARB.ArrayBuffer;
                PieMetrics.VertexBufferCount++;
                break;
            case BufferType.IndexBuffer:
                target = BufferTargetARB.ElementArrayBuffer;
                PieMetrics.IndexBufferCount++;
                break;
            case BufferType.UniformBuffer:
                target = BufferTargetARB.UniformBuffer;
                PieMetrics.UniformBufferCount++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        BufferUsageARB usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        uint handle = Gl.GenBuffer();
        Gl.BindBuffer(target, handle);
        Gl.BufferData(target, sizeInBytes, data, usage);

        return new OpenGL33GraphicsBuffer(handle, target);
    }

    public unsafe void Update(uint offsetInBytes, uint sizeInBytes, void* data)
    {
        Gl.BindBuffer(Target, Handle);
        Gl.BufferSubData(Target, (nint) offsetInBytes, (nuint) sizeInBytes, data);
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
            case BufferTargetARB.UniformBuffer:
                PieMetrics.UniformBufferCount--;
                break;
        }
    }
}