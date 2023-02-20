using System;

namespace Pie;

/// <summary>
/// Flags to describe how a texture will be used.
/// </summary>
[Flags]
public enum TextureUsage
{
    /// <summary>
    /// This texture is not used.
    /// </summary>
    None = 1 << 0,
    
    /// <summary>
    /// This texture is used as a shader resource.
    /// </summary>
    ShaderResource = 1 << 1,
    
    /// <summary>
    /// This texture is used as a framebuffer.
    /// </summary>
    Framebuffer = 1 << 2,
}