namespace Pie.Graphics;

public enum TextureSample
{
    /// <summary>
    /// Use linear sampling for this texture.
    /// </summary>
    Linear,
    
    /// <summary>
    /// Use nearest sampling for this texture. (Equivalent to <see cref="Point"/>.)
    /// </summary>
    Nearest,
    
    /// <summary>
    /// Use point sampling for this texture. (Equivalent to <see cref="Nearest"/>.)
    /// </summary>
    Point = Nearest
}