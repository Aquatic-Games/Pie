namespace Pie;

/// <summary>
/// Used to define what is sampled when the texture coordinates go outside the 0-1 range.
/// </summary>
public enum TextureAddress
{
    /// <summary>
    /// The texture will repeat.
    /// </summary>
    Repeat,
    
    /// <summary>
    /// The texture will mirror.
    /// </summary>
    Mirror,
    
    /// <summary>
    /// The texture will clamp to the edge (the last row of pixels).
    /// </summary>
    ClampToEdge,
    
    /// <summary>
    /// The texture will clamp to a border color.
    /// </summary>
    ClampToBorder
}