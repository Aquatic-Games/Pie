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
    private BufferType _type;

    private bool _dynamic;

    public D3D11GraphicsBuffer(ID3D11Buffer buffer, bool dynamic, BufferType type)
    {
        Buffer = buffer;
        _dynamic = dynamic;
        _type = type;
    }

    public static unsafe GraphicsBuffer CreateBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        BindFlags flags;

        switch (type)
        {
            case BufferType.VertexBuffer:
                flags = BindFlags.VertexBuffer;
                PieMetrics.VertexBufferCount++;
                break;
            case BufferType.IndexBuffer:
                flags = BindFlags.IndexBuffer;
                PieMetrics.IndexBufferCount++;
                break;
            case BufferType.UniformBuffer:
                flags = BindFlags.ConstantBuffer;
                PieMetrics.UniformBufferCount++;
                break;
            case BufferType.ShaderStorageBuffer:
                flags = BindFlags.ShaderResource;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        BufferDescription description = new BufferDescription()
        {
            BindFlags = flags,
            ByteWidth = (int) sizeInBytes,
            Usage = dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
            CPUAccessFlags = dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None
        };

        if (type == BufferType.ShaderStorageBuffer)
        {
            description.MiscFlags = ResourceOptionFlags.BufferStructured;
            description.StructureByteStride = 1;
        }
        
        ID3D11Buffer buffer;
        if (data == null)
            buffer = Device.CreateBuffer(description);
        else
            buffer = Device.CreateBuffer(description, new SubresourceData(data));
        return new D3D11GraphicsBuffer(buffer, dynamic, type);
    }
    
    public unsafe void Update(uint offsetInBytes, uint sizeInBytes, void* data)
    {
        if (_dynamic)
        {
            // TODO check to make sure WriteDiscard works correctly
            
            // Thanks to veldrid source for helping me understand this mess
            MappedSubresource subresource = Context.Map(Buffer, Vortice.Direct3D11.MapMode.WriteDiscard);
            Unsafe.CopyBlock((byte*) subresource.DataPointer + (int) offsetInBytes, data, sizeInBytes);
            Context.Unmap(Buffer);
        }
        else
        {
            Context.UpdateSubresource(Buffer, 0,
                new Box((int) offsetInBytes, 0, 0, (int) (sizeInBytes + offsetInBytes), 1, 1), (IntPtr) data, 0, 0);
        }
    }

    /*public unsafe void Update<T>(uint offsetInBytes, T data) where T : unmanaged
    {
        // While these two functions are duplicates it avoids creating an array every time update is called.
        if (_dynamic)
        {
            MappedSubresource subresource = Context.Map(Buffer, Vortice.Direct3D11.MapMode.WriteDiscard);
            Unsafe.CopyBlock((byte*) subresource.DataPointer + (int) offsetInBytes,&data, (uint) Unsafe.SizeOf<T>());
            Context.Unmap(Buffer);
        }
        else
        {
            Context.UpdateSubresource(data, Buffer,
                region: new Box((int) offsetInBytes, 0, 0, (int) (Unsafe.SizeOf<T>() + offsetInBytes),
                    1, 1));
        }
    }*/

    public override void Dispose()
    {
        Buffer.Dispose();
        switch (_type)
        {
            case BufferType.VertexBuffer:
                PieMetrics.VertexBufferCount--;
                break;
            case BufferType.IndexBuffer:
                PieMetrics.IndexBufferCount--;
                break;
            case BufferType.UniformBuffer:
                PieMetrics.UniformBufferCount--;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}