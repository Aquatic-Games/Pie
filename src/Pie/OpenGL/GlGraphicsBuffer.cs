using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly int Handle;
    public readonly BufferTarget Target;
    public readonly uint SizeInBytes;

    public unsafe GlGraphicsBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        SizeInBytes = sizeInBytes;
        
        switch (type)
        {
            case BufferType.VertexBuffer:
                Target = BufferTarget.ArrayBuffer;
                PieMetrics.VertexBufferCount++;
                break;
            case BufferType.IndexBuffer:
                Target = BufferTarget.ElementArrayBuffer;
                PieMetrics.IndexBufferCount++;
                break;
            case BufferType.UniformBuffer:
                Target = BufferTarget.UniformBuffer;
                PieMetrics.UniformBufferCount++;
                break;
            case BufferType.ShaderStorageBuffer:
                Target = BufferTarget.ShaderStorageBuffer;
                // TODO: Shader storage buffer count?
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        BufferUsageHint usage = dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw;

        Handle = GL.GenBuffer();
        GL.BindBuffer(Target, Handle);
        GL.BufferData(Target, (int) sizeInBytes, (IntPtr) data, usage);
    }

    public unsafe void Update(uint offsetInBytes, uint sizeInBytes, void* data)
    {
        GL.BindBuffer(Target, Handle);
        GL.BufferSubData(Target, (IntPtr) offsetInBytes, (int) sizeInBytes, (IntPtr) data);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        GL.DeleteBuffer(Handle);
        switch (Target)
        {
            case BufferTarget.ArrayBuffer:
                PieMetrics.VertexBufferCount--;
                break;
            case BufferTarget.ElementArrayBuffer:
                PieMetrics.IndexBufferCount--;
                break;
            case BufferTarget.UniformBuffer:
                PieMetrics.UniformBufferCount--;
                break;
        }
    }

    internal override unsafe MappedSubresource Map(MapMode mode)
    {
        GL.BindBuffer(Target, Handle);
        IntPtr mapped = GL.MapBufferRange(Target, IntPtr.Zero, (int) SizeInBytes, (BufferAccessMask) mode.ToGlMapMode());

        return new MappedSubresource(mapped);
    }

    internal override void Unmap()
    {
        GL.UnmapBuffer(Target);
    }
}