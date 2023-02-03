namespace Pie;

/// <summary>
/// Describes how a <see cref="BlendState"/> should behave.
/// </summary>
public struct BlendStateDescription
{
    /// <summary>
    /// Use non-premultiplied alpha.
    /// </summary>
    public static readonly BlendStateDescription NonPremultiplied =
        new BlendStateDescription(BlendType.SrcAlpha, BlendType.OneMinusSrcAlpha);

    /// <summary>
    /// Use alpha blending.
    /// </summary>
    public static readonly BlendStateDescription
        AlphaBlend = new BlendStateDescription(BlendType.One, BlendType.OneMinusSrcAlpha);

    /// <summary>
    /// Use additive blending.
    /// </summary>
    public static readonly BlendStateDescription Additive = new BlendStateDescription(BlendType.SrcAlpha, BlendType.One);

    /// <summary>
    /// Use opaque blending.
    /// </summary>
    public static readonly BlendStateDescription Opaque = new BlendStateDescription(BlendType.One, BlendType.Zero);
    
    /// <summary>
    /// The source blending type.
    /// </summary>
    public BlendType Source;
    
    /// <summary>
    /// The destination blending type.
    /// </summary>
    public BlendType Destination;
    
    /// <summary>
    /// Create a new blend state description.
    /// </summary>
    /// <param name="source">The source blending type.</param>
    /// <param name="destination">The destination blending type.</param>
    public BlendStateDescription(BlendType source, BlendType destination)
    {
        // TODO: Blend function?
        Source = source;
        Destination = destination;
    }
}