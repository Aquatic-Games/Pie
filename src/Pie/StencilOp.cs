namespace Pie;

/// <summary>
/// Various operations that can occur during stencil testing.
/// </summary>
public enum StencilOp
{
    /// <summary>
    /// Keep the existing stencil data.
    /// </summary>
    Keep,
    
    /// <summary>
    /// Set the stencil data to 0.
    /// </summary>
    Zero,
    
    /// <summary>
    /// Replace the existing stencil data to the reference value.
    /// </summary>
    Replace,
    
    /// <summary>
    /// Increment the stencil value by 1, and clamp the result.
    /// </summary>
    Increment,
    
    /// <summary>
    /// Increment the stencil value by 1, and wrap the result if necessary.
    /// </summary>
    IncrementWrap,
    
    /// <summary>
    /// Decrement the stencil value by 1, and clamp the result.
    /// </summary>
    Decrement,
    
    /// <summary>
    /// Decrement the stencil value by 1, and wrap the result if necessary.
    /// </summary>
    DecrementWrap,
    
    /// <summary>
    /// Invert the stencil data.
    /// </summary>
    Invert
}