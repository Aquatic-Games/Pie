namespace Pie;

/// <summary>
/// Describes when the various tests should pass.
/// </summary>
public enum ComparisonFunc
{
    /// <summary>
    /// The test never passes.
    /// </summary>
    Never,
    
    /// <summary>
    /// The test passes if the incoming value is less than the stored value.
    /// </summary>
    Less,
    
    /// <summary>
    /// The test passes if the incoming value is equal to the stored value.
    /// </summary>
    Equal,
    
    /// <summary>
    /// The test passes if the incoming value is less than or equal to the stored value.
    /// </summary>
    LessEqual,
    
    /// <summary>
    /// The test passes if the incoming value is greater than the stored value.
    /// </summary>
    Greater,
    
    /// <summary>
    /// The test passes if the incoming value is not equal to the stored value.
    /// </summary>
    NotEqual,
    
    /// <summary>
    /// The test passes if the incoming value is greater than or equal to the stored value.
    /// </summary>
    GreaterEqual,
    
    /// <summary>
    /// The test will always pass.
    /// </summary>
    Always
}