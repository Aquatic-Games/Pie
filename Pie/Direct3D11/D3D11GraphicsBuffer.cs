using System;
using System.Runtime.CompilerServices;
using Vortice.Direct3D11;
using Vortice.Mathematics;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11GraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public ID3D11Buffer Buffer;

    private bool _dynamic;

    public D3D11GraphicsBuffer(ID3D11Buffer buffer, bool dynamic)
    {
        Buffer = buffer;
        _dynamic = dynamic;
    }

    public static GraphicsBuffer CreateBuffer<T>(BufferType type, uint sizeInBytes, T[] data, bool dynamic) where T : unmanaged
    {
        BindFlags flags = type switch
        {
            BufferType.VertexBuffer => BindFlags.VertexBuffer,
            BufferType.IndexBuffer => BindFlags.IndexBuffer,
            BufferType.UniformBuffer => BindFlags.ConstantBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        BufferDescription description = new BufferDescription()
        {
            BindFlags = flags,
            ByteWidth = (int) sizeInBytes,
            Usage = dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
            CPUAccessFlags = dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None
        };
        
        return new D3D11GraphicsBuffer(Device.CreateBuffer(new ReadOnlySpan<T>(data), description), dynamic);
    }
    
    public unsafe void Update<T>(uint offsetInBytes, T[] data) where T : unmanaged
    {
        if (_dynamic)
        {
            MappedSubresource subresource = Context.Map(Buffer, MapMode.WriteDiscard);
            Unsafe.Copy((subresource.DataPointer + (int) offsetInBytes).ToPointer(), ref data);
            Context.Unmap(Buffer);
        }
        else
        {
            Context.UpdateSubresource(data, Buffer,
                region: new Box((int) offsetInBytes, 0, 0, (int) ((Unsafe.SizeOf<T>() * data.Length) + offsetInBytes),
                    1, 1));
        }
    }

    public unsafe void Update<T>(uint offsetInBytes, T data) where T : unmanaged
    {
        // While these two functions are duplicates it avoids creating an array every time update is called.
        if (_dynamic)
        {
            MappedSubresource subresource = Context.Map(Buffer, MapMode.WriteDiscard);
            Unsafe.Copy((subresource.DataPointer + (int) offsetInBytes).ToPointer(), ref data);
            Context.Unmap(Buffer);
        }
        else
        {
            Context.UpdateSubresource(data, Buffer,
                region: new Box((int) offsetInBytes, 0, 0, (int) (Unsafe.SizeOf<T>() + offsetInBytes),
                    1, 1));
        }
    }

    public override void Dispose()
    {
        Buffer.Dispose();
    }
}