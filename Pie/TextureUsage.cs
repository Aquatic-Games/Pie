using System;

namespace Pie;

[Flags]
public enum TextureUsage
{
    None = 1 << 0,
    
    ShaderResource = 1 << 1,
    
    Framebuffer = 1 << 2,
}