namespace Pie;

/// <summary>
/// Describes how a <see cref="BlendState"/> should behave.
/// </summary>
public struct BlendStateDescription
{
    /// <summary>
    /// Disable blending.
    /// </summary>
    public static readonly BlendStateDescription Disabled = new BlendStateDescription(false, BlendType.One,
        BlendType.Zero, BlendOperation.Add, BlendType.One, BlendType.Zero, BlendOperation.Add, ColorWriteMask.All);
    
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
    /// Whether or not blending is enabled.
    /// </summary>
    public bool Enabled;
    
    /// <summary>
    /// The RGB source blending type.
    /// </summary>
    public BlendType Source;
    
    /// <summary>
    /// The RGB destination blending type.
    /// </summary>
    public BlendType Destination;

    /// <summary>
    /// The <see cref="Pie.BlendOperation"/> to perform between the <see cref="Source"/> and the <see cref="Destination"/>.
    /// </summary>
    public BlendOperation BlendOperation;

    /// <summary>
    /// The alpha source blending type.
    /// </summary>
    public BlendType SourceAlpha;

    /// <summary>
    /// The alpha destination blending type.
    /// </summary>
    public BlendType DestinationAlpha;

    /// <summary>
    /// The <see cref="Pie.BlendOperation"/> to perform between the <see cref="SourceAlpha"/> and the <see cref="DestinationAlpha"/>.
    /// </summary>
    public BlendOperation AlphaBlendOperation;

    /// <summary>
    /// The write mask to use.
    /// </summary>
    public ColorWriteMask ColorWriteMask;

    /// <summary>
    /// Create a new <see cref="BlendStateDescription"/>.
    /// </summary>
    /// <param name="enabled">Whether or not blending is enabled.</param>
    /// <param name="source">The RGB source blending type.</param>
    /// <param name="destination">The RGB destination blending type.</param>
    /// <param name="blendOperation">The <see cref="Pie.BlendOperation"/> to perform between the <see cref="Source"/> and the <see cref="Destination"/>.</param>
    /// <param name="sourceAlpha">The alpha source blending type.</param>
    /// <param name="destinationAlpha">The alpha destination blending type.</param>
    /// <param name="alphaBlendOperation">The <see cref="Pie.BlendOperation"/> to perform between the <see cref="SourceAlpha"/> and the <see cref="DestinationAlpha"/>.</param>
    /// <param name="colorWriteMask">The write mask to use.</param>
    public BlendStateDescription(bool enabled, BlendType source, BlendType destination, BlendOperation blendOperation,
        BlendType sourceAlpha, BlendType destinationAlpha, BlendOperation alphaBlendOperation, ColorWriteMask colorWriteMask)
    {
        Enabled = enabled;
        Source = source;
        Destination = destination;
        BlendOperation = blendOperation;
        SourceAlpha = sourceAlpha;
        DestinationAlpha = destinationAlpha;
        AlphaBlendOperation = alphaBlendOperation;
        ColorWriteMask = colorWriteMask;
    }
    
    /// <summary>
    /// Create a new <see cref="BlendStateDescription"/>.
    /// </summary>
    /// <param name="source">The RGBA source blending type.</param>
    /// <param name="destination">The RGBA destination blending type.</param>
    public BlendStateDescription(BlendType source, BlendType destination) : this(source, destination, source,
        destination) { }

    /// <summary>
    /// Create a new <see cref="BlendStateDescription"/>.
    /// </summary>
    /// <param name="source">The RGB source blending type.</param>
    /// <param name="destination">The RGB destination blending type.</param>
    /// <param name="sourceAlpha">The alpha source blending type.</param>
    /// <param name="destinationAlpha">The alpha destination blending type.</param>
    public BlendStateDescription(BlendType source, BlendType destination, BlendType sourceAlpha,
        BlendType destinationAlpha) : this(true, source, destination, BlendOperation.Add, sourceAlpha, destinationAlpha,
        BlendOperation.Add, ColorWriteMask.All) { }
}