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
            attributeDescriptions[i].Format = descriptions[i].Type switch
            {
                AttributeType.Int => Format.R32Sint,
                AttributeType.Int2 => Format.R32G32Sint,
                AttributeType.Int3 => Format.R32G32B32Sint,
                AttributeType.Int4 => Format.R32G32B32A32Sint,
                AttributeType.Float => Format.R32Sfloat,
                AttributeType.Float2 => Format.R32G32Sfloat,
                AttributeType.Float3 => Format.R32G32B32Sfloat,
                AttributeType.Float4 => Format.R32G32B32A32Sfloat,
                AttributeType.Byte => Format.R8Uint,
                AttributeType.Byte2 => Format.R8G8Uint,
                AttributeType.Byte4 => Format.R8G8B8A8Uint,
                AttributeType.NByte => Format.R8Unorm,
                AttributeType.NByte2 => Format.R8G8Unorm,
                AttributeType.NByte4 => Format.R8G8B8A8Unorm,
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