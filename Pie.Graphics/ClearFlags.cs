using System;

namespace Pie.Graphics;

[Flags]
public enum ClearFlags
{
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