using System;

namespace Pie;

/// <summary>
/// The color write mask tells the graphics device which color channels to draw to the current framebuffer.
/// </summary>
[Flags]
public enum ColorWriteMask
{
    /// <summary>
    /// Draw to no channels.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Draw to the red channel.
    /// </summary>
    Red = 1 << 0,
    
    /// <summary>
    /// Draw to the green channel.
    /// </summary>
    Green = 1 << 1,
    
    /// <summary>
    /// Draw to the blue channel.
    /// </summary>
    Blue = 1 << 2,
    
    /// <summary>
    /// Draw to the alpha channel.
    /// </summary>
    Alpha = 1 << 3,
    
    /// <summary>
    /// Draw to all channels (the default behavior).
    /// </summary>
    All = Red | Green | Blue | Alpha
}