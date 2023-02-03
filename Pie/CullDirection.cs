namespace Pie;

/// <summary>
/// The direction to use during culling. This is typically equal to the winding order of the vertices.
/// </summary>
public enum CullDirection
{
    /// <summary>
    /// Cull clockwise.
    /// </summary>
    Clockwise,
    
    /// <summary>
    /// Cull counter clockwise. (Equivalent to <see cref="AntiClockwise"/>.)
    /// </summary>
    CounterClockwise,
    
    /// <summary>
    /// Cull anti clockwise. (Equivalent to <see cref="AntiClockwise"/>.)
    /// </summary>
    AntiClockwise = 1
}