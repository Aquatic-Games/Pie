using System;
using System.Numerics;

namespace Pie.Vulkan;

internal sealed class VkShader : Shader
{
    public override bool IsDisposed { get; protected set; }
    
    public override void Set(string name, bool value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, int value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, float value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Vector2 value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Vector3 value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Vector4 value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Matrix4x4 value, bool transpose = true)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Vulkan. You must create a uniform/constant buffer instead.");
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}