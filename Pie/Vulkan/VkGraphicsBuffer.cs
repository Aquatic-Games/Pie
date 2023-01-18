using System;
using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Pie.Vulkan;

internal sealed unsafe class VkGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public VkGraphicsBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        // TODO: Dynamic buffers?
        
        Vk vk = VkHelper.VK;
        ref Device device = ref VkHelper.Device;
        
        BufferCreateInfo bfi = new BufferCreateInfo();
        bfi.SType = StructureType.BufferCreateInfo;
        bfi.Size = sizeInBytes;
        bfi.Usage = BufferUsageFlags.TransferSrcBit;

        Result result;
        
        Buffer staging;
        if ((result = vk.CreateBuffer(device, &bfi, null, &staging)) != Result.Success)
            throw new PieException("Failed to create staging buffer: " + result);

        MemoryRequirements requirements;
        vk.GetBufferMemoryRequirements(device, staging, &requirements);

        MemoryAllocateInfo allocInfo = new MemoryAllocateInfo();
        allocInfo.SType = StructureType.MemoryAllocateInfo;
        allocInfo.AllocationSize = requirements.Size;
        allocInfo.MemoryTypeIndex = VkHelper.GetMemoryTypeIndex(requirements.MemoryTypeBits,
            MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);

        DeviceMemory stagingMemory;
        if ((result = vk.AllocateMemory(device, &allocInfo, null, &stagingMemory)) != Result.Success)
            throw new PieException("Failed to allocate staging buffer memory: " + result);

        void* stagingData;
        if ((result = vk.MapMemory(device, stagingMemory, 0, allocInfo.AllocationSize, 0, &stagingData)) != Result.Success)
            throw new PieException("Failed to map staging memory: " + result);
        
        Unsafe.CopyBlock(stagingData, data, sizeInBytes);
        
        vk.UnmapMemory(device, stagingMemory);

        if ((result = vk.BindBufferMemory(device, staging, stagingMemory, 0)) != Result.Success)
            throw new PieException("Failed to bind staging memory to staging buffer: " + result);

        BufferUsageFlags bufferUsage = type switch
        {
            BufferType.VertexBuffer => BufferUsageFlags.VertexBufferBit,
            BufferType.IndexBuffer => BufferUsageFlags.IndexBufferBit,
            BufferType.UniformBuffer => BufferUsageFlags.UniformBufferBit,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        bfi.Usage = bufferUsage | BufferUsageFlags.TransferDstBit;

        Buffer buffer;
        if ((result = vk.CreateBuffer(device, &bfi, null, &buffer)) != Result.Success)
            throw new PieException("Failed to create " + type + " buffer: " + result);
        
        vk.GetBufferMemoryRequirements(device, buffer, &requirements);
        allocInfo.AllocationSize = requirements.Size;
        allocInfo.MemoryTypeIndex =
            VkHelper.GetMemoryTypeIndex(requirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit);

        DeviceMemory bufferMemory;
        if ((result = vk.AllocateMemory(device, &allocInfo, null, &bufferMemory)) != Result.Success)
            throw new PieException("Failed to allocate " + type + " buffer memory: " + result);

        if ((result = vk.BindBufferMemory(device, buffer, bufferMemory, 0)) != Result.Success)
            throw new PieException("Failed to bind " + type + " buffer memory: " + result);

        CommandBuffer copyBuffer = VkHelper.CreateCommandBuffer(true);
        BufferCopy copyRegion = new BufferCopy();
        copyRegion.Size = sizeInBytes;
        vk.CmdCopyBuffer(copyBuffer, staging, buffer, 1, &copyRegion);
        
        vk.DestroyBuffer(device, staging, null);
        vk.FreeMemory(device, stagingMemory, null);
        
        Console.WriteLine("Created " + type + " buffer successfully.");
    }
    
    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}