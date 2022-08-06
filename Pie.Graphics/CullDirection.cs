namespace Pie.Graphics;

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