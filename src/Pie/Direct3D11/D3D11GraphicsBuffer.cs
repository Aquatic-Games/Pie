using System;
using System.Runtime.CompilerServices;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11GraphicsBuffer : GraphicsBuffer
{
    private ComPtr<ID3D11DeviceContext> _context;
    private BufferType _type;
    private bool _dynamic;
    
    public override bool IsDisposed { get; protected set; }

    public ComPtr<ID3D11Buffer> Buffer;

    public D3D11GraphicsBuffer(ComPtr<ID3D11Device> device, ComPtr<ID3D11DeviceContext> context, BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        _context = context;
        
        _dynamic = dynamic;
        _type = type;
        
        BindFlag flags;

        switch (type)
        {
            case BufferType.VertexBuffer:
                flags = BindFlag.VertexBuffer;
                PieMetrics.VertexBufferCount++;
                break;
            case BufferType.IndexBuffer:
                flags = BindFlag.IndexBuffer;
                PieMetrics.IndexBufferCount++;
                break;
            case BufferType.UniformBuffer:
                flags = BindFlag.ConstantBuffer;
                PieMetrics.UniformBufferCount++;
                break;
            case BufferType.ShaderStorageBuffer:
                flags = BindFlag.ShaderResource;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        BufferDesc description = new BufferDesc()
        {
            BindFlags = (uint) flags,
            ByteWidth = sizeInBytes,
            Usage = dynamic ? Usage.Dynamic : Usage.Default,
            CPUAccessFlags = (uint) (dynamic ? CpuAccessFlag.Write : CpuAccessFlag.None)
        };

        if (type == BufferType.ShaderStorageBuffer)
        {
            description.MiscFlags = (uint) ResourceMiscFlag.BufferStructured;
            description.StructureByteStride = 1;
        }

        SubresourceData subData = new SubresourceData(data);

        if (!Succeeded(device.CreateBuffer(&description, data == null ? null : &subData, ref Buffer)))
            throw new PieException("Failed to create buffer.");
    }

    public unsafe void Update(uint offsetInBytes, uint sizeInBytes, void* data)
    {
        if (_dynamic)
        {
            Silk.NET.Direct3D11.MappedSubresource subresource = default;
            _context.Map(Buffer, 0, Silk.NET.Direct3D11.Map.WriteDiscard, 0, ref subresource);
            Unsafe.CopyBlock((byte*) subresource.PData + (int) offsetInBytes, data, sizeInBytes);
            _context.Unmap(Buffer, 0);
        }
        else
        {
            _context.UpdateSubresource(Buffer, 0, new Box(offsetInBytes, 0, 0, sizeInBytes + offsetInBytes, 1, 1), data,
                0, 0);
        }
    }

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

    internal override MappedSubresource Map(MapMode mode)
    {
        Silk.NET.Direct3D11.MappedSubresource resource = default;
        if (!Succeeded(_context.Map(Buffer, 0, mode.ToDx11MapMode(), 0, ref resource)))
            throw new PieException("Failed to map resource.");
        void* data = resource.PData;

        return new MappedSubresource((IntPtr) data);
    }

    internal override void Unmap()
    {
        _context.Unmap(Buffer, 0);
    }
}