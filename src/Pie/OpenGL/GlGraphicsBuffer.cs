using System;
using static Pie.OpenGL.GlGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GlGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly uint Handle;
    public readonly BufferTargetARB Target;
    public readonly uint SizeInBytes;

    public unsafe GlGraphicsBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        SizeInBytes = sizeInBytes;
        
        switch (type)
        {
            case BufferType.VertexBuffer:
                Target = BufferTargetARB.ArrayBuffer;
                PieMetrics.VertexBufferCount++;
                break;
            case BufferType.IndexBuffer:
                Target = BufferTargetARB.ElementArrayBuffer;
                PieMetrics.IndexBufferCount++;
                break;
            case BufferType.UniformBuffer:
                Target = BufferTargetARB.UniformBuffer;
                PieMetrics.UniformBufferCount++;
                break;
            case BufferType.ShaderStorageBuffer:
                Target = BufferTargetARB.ShaderStorageBuffer;
                // TODO: Shader storage buffer count?
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        BufferUsageARB usage = dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw;

        Handle = Gl.GenBuffer();
        Gl.BindBuffer(Target, Handle);
        Gl.BufferData(Target, sizeInBytes, data, usage);
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

    internal override unsafe MappedSubresource Map(MapMode mode)
    {
        Gl.BindBuffer(Target, Handle);
        void* mapped = Gl.MapBufferRange(Target, 0, SizeInBytes, mode.ToGlMapMode());

        return new MappedSubresource((IntPtr) mapped);
    }

    internal override void Unmap()
    {
        Gl.UnmapBuffer(Target);
    }
}