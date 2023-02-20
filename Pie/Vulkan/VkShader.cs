using System;
using System.Numerics;

namespace Pie.Vulkan;

internal sealed class VkShader : Shader
{
    public override bool IsDisposed { get; protected set; }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}