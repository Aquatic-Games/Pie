using System;

namespace Pie;

[Flags]
public enum ClearFlags
{
    /// <summary>
    /// Clear the color bit.
    /// </summary>
    Color = 1 << 0,
    
    /// <summary>
    /// Clear the depth bit.
    /// </summary>
    Depth = 1 << 1,
    
    /// <summary>
    /// Clear the stencil bit.
    /// </summary>
    Stencil = 1 << 2
}