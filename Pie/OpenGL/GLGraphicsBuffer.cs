using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GLGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly uint Handle;
    public readonly BufferTargetARB Target;

    public GLGraphicsBuffer(uint handle, BufferTargetARB target)
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
            case BufferType.ShaderStorageBuffer:
                target = BufferTargetARB.ShaderStorageBuffer;
                // TODO: Shader storage buffer count?
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        BufferUsageARB usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        uint handle = Gl.GenBuffer();
        Gl.BindBuffer(target, handle);
        Gl.BufferData(target, sizeInBytes, data, usage);

        return new GLGraphicsBuffer(handle, target);
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