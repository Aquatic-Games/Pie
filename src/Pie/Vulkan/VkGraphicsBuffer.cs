using System;
using Silk.NET.Vulkan;
using static Pie.Vulkan.VkLayer;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Pie.Vulkan;

internal sealed unsafe class VkGraphicsBuffer : GraphicsBuffer
{
    private Device _device;

    private DeviceMemory _memory;
    
    public readonly Buffer Buffer;
    
    public override bool IsDisposed { get; protected set; }

    public VkGraphicsBuffer(VkLayer layer, in VkDevice device, BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        _device = device.Device;
        
        BufferUsageFlags usage = type switch
        {
            BufferType.VertexBuffer => BufferUsageFlags.VertexBufferBit,
            BufferType.IndexBuffer => BufferUsageFlags.IndexBufferBit,
            BufferType.UniformBuffer => BufferUsageFlags.UniformBufferBit,
            BufferType.ShaderStorageBuffer => BufferUsageFlags.StorageBufferBit,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        BufferCreateInfo bufferCreateInfo = new BufferCreateInfo()
        {
            SType = StructureType.BufferCreateInfo,
            Size = sizeInBytes,
            Usage = usage,
            SharingMode = SharingMode.Exclusive
        };
        
        CheckResult(VK.CreateBuffer(_device, &bufferCreateInfo, null, out Buffer));

        MemoryRequirements requirements;
        VK.GetBufferMemoryRequirements(_device, Buffer, &requirements);

        MemoryPropertyFlags flags = dynamic
            ? MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit
            : MemoryPropertyFlags.DeviceLocalBit;

        uint memoryType = layer.GetSuitableMemoryType(device, requirements, flags);

        MemoryAllocateInfo allocateInfo = new MemoryAllocateInfo()
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size,
            MemoryTypeIndex = memoryType
        };
        
        CheckResult(VK.AllocateMemory(_device, &allocateInfo, null, out _memory));
        
        CheckResult(VK.BindBufferMemory(_device, Buffer, _memory, 0));
    }
    
    internal override MappedSubresource Map(MapMode mode)
    {
        throw new System.NotImplementedException();
    }

    internal override void Unmap()
    {
        throw new System.NotImplementedException();
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
        
        VK.DestroyBuffer(_device, Buffer, null);
        VK.FreeMemory(_device, _memory, null);
    }
}