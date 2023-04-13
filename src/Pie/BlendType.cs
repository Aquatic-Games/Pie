namespace Pie;

/// <summary>
/// The blending type for a source or destination.
/// </summary>
public enum BlendType
{
    /// <summary>
    /// Equal to 0.
    /// </summary>
    Zero,
    
    /// <summary>
    /// Equal to 1.
    /// </summary>
    One,
    
    /// <summary>
    /// Equal to the source color.
    /// </summary>
    SrcColor,
    
    /// <summary>
    /// Equal to 1 minus the source color.
    /// </summary>
    OneMinusSrcColor,
    
    /// <summary>
    /// Equal to the destination color.
    /// </summary>
    DestColor,
    
    /// <summary>
    /// Equal to 1 minus the destination color.
    /// </summary>
    OneMinusDestColor,
    
    /// <summary>
    /// Equal to the source alpha.
    /// </summary>
    SrcAlpha,
    
    /// <summary>
    /// Equal to 1 minus the source alpha.
    /// </summary>
    OneMinusSrcAlpha,
    
    /// <summary>
    /// Equal to the destination alpha.
    /// </summary>
    DestAlpha,
    
    /// <summary>
    /// Equal to 1 minus the destination alpha.
    /// </summary>
    OneMinusDestAlpha
}