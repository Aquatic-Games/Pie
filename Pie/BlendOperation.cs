namespace Pie;

/// <summary>
/// A blending operation.
/// </summary>
public enum BlendOperation
{
    /// <summary>
    /// Add source 1 and source 2 together.
    /// </summary>
    Add,
    
    /// <summary>
    /// Subtract source 1 from source 2.
    /// </summary>
    Subtract,
    
    /// <summary>
    /// Subtract source 2 from source 1.
    /// </summary>
    ReverseSubtract,
    
    /// <summary>
    /// The minimum of source 1 and 2.
    /// </summary>
    Min,
    
    /// <summary>
    /// The maximum of source 1 and 2.
    /// </summary>
    Max
}