using System;
using Silk.NET.Vulkan;

namespace Pie.Vulkan;

internal sealed class VulkanInputLayout : InputLayout
{
    public override bool IsDisposed { get; protected set; }
    
    public override InputLayoutDescription[] Descriptions { get; }

    public VulkanInputLayout(InputLayoutDescription[] descriptions)
    {
        Descriptions = descriptions;

        VertexInputAttributeDescription[] attributeDescriptions = new VertexInputAttributeDescription[descriptions.Length];

        for (int i = 0; i < descriptions.Length; i++)
        {
            attributeDescriptions[i].Binding = descriptions[i].Slot;
            attributeDescriptions[i].Location = (uint) i;
            attributeDescriptions[i].Format = descriptions[i].Format switch
            {
                Format.R8_UNorm => Silk.NET.Vulkan.Format.R8Unorm,
                Format.R8G8_UNorm => Silk.NET.Vulkan.Format.R8G8Unorm,
                Format.R8G8B8A8_UNorm => Silk.NET.Vulkan.Format.R8G8B8A8Unorm,
                Format.B8G8R8A8_UNorm => Silk.NET.Vulkan.Format.B8G8R8A8Unorm,
                Format.R16G16B16A16_UNorm => Silk.NET.Vulkan.Format.R16G16B16A16Unorm,
                Format.R16G16B16A16_SNorm => Silk.NET.Vulkan.Format.R16G16B16A16SNorm,
                Format.R16G16B16A16_SInt => Silk.NET.Vulkan.Format.R16G16B16A16Sint,
                Format.R16G16B16A16_UInt => Silk.NET.Vulkan.Format.R16G16B16A16Uint,
                Format.R16G16B16A16_Float => Silk.NET.Vulkan.Format.R16G16B16A16Sfloat,
                Format.R32G32_SInt => Silk.NET.Vulkan.Format.R32G32Sint,
                Format.R32G32_UInt => Silk.NET.Vulkan.Format.R32G32Uint,
                Format.R32G32_Float => Silk.NET.Vulkan.Format.R32G32Sfloat,
                Format.R32G32B32_SInt => Silk.NET.Vulkan.Format.R32G32B32Sint,
                Format.R32G32B32_UInt => Silk.NET.Vulkan.Format.R32G32B32Uint,
                Format.R32G32B32_Float => Silk.NET.Vulkan.Format.R32G32B32Sfloat,
                Format.R32G32B32A32_SInt => Silk.NET.Vulkan.Format.R32G32B32A32Sint,
                Format.R32G32B32A32_UInt => Silk.NET.Vulkan.Format.R32G32B32A32Uint,
                Format.R32G32B32A32_Float => Silk.NET.Vulkan.Format.R32G32B32A32Sfloat,
                Format.D24_UNorm_S8_UInt => Silk.NET.Vulkan.Format.D24UnormS8Uint,
                Format.R8_SNorm => Silk.NET.Vulkan.Format.R8SNorm,
                Format.R8_SInt => Silk.NET.Vulkan.Format.R8Sint,
                Format.R8_UInt => Silk.NET.Vulkan.Format.R8Uint,
                Format.R8G8_SNorm => Silk.NET.Vulkan.Format.R8G8SNorm,
                Format.R8G8_SInt => Silk.NET.Vulkan.Format.R8G8Sint,
                Format.R8G8_UInt => Silk.NET.Vulkan.Format.R8G8Uint,
                Format.R8G8B8A8_SNorm => Silk.NET.Vulkan.Format.R8G8B8A8SNorm,
                Format.R8G8B8A8_SInt => Silk.NET.Vulkan.Format.R8G8B8A8Sint,
                Format.R8G8B8A8_UInt => Silk.NET.Vulkan.Format.R8G8B8A8Uint,
                Format.R16_UNorm => Silk.NET.Vulkan.Format.R16Unorm,
                Format.R16_SNorm => Silk.NET.Vulkan.Format.R16SNorm,
                Format.R16_SInt => Silk.NET.Vulkan.Format.R16Sint,
                Format.R16_UInt => Silk.NET.Vulkan.Format.R16Uint,
                Format.R16_Float => Silk.NET.Vulkan.Format.R16Sfloat,
                Format.R16G16_UNorm => Silk.NET.Vulkan.Format.R16G16Unorm,
                Format.R16G16_SNorm => Silk.NET.Vulkan.Format.R16G16SNorm,
                Format.R16G16_SInt => Silk.NET.Vulkan.Format.R16G16Sint,
                Format.R16G16_UInt => Silk.NET.Vulkan.Format.R16G16Uint,
                Format.R16G16_Float => Silk.NET.Vulkan.Format.R16G16Sfloat,
                Format.R32_SInt => Silk.NET.Vulkan.Format.R32Sint,
                Format.R32_UInt => Silk.NET.Vulkan.Format.R32Uint,
                Format.R32_Float => Silk.NET.Vulkan.Format.R32Sfloat,
                _ => throw new ArgumentOutOfRangeException()
            };
            attributeDescriptions[i].Offset = descriptions[i].Offset;
        }
    }
    
    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}