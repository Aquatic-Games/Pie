using System;

namespace Pie;

/// <summary>
/// The flags to use when clearing the target framebuffer.
/// </summary>
[Flags]
public enum ClearFlags
{
    /// <summary>
    /// Do not clear anything.
    /// </summary>
    None,
    
    /// <summary>
    /// Clear the depth bit.
    /// </summary>
    Depth = 1 << 0,
    
    /// <summary>
    /// Clear the stencil bit.
    /// </summary>
    Stencil = 1 << 1
}