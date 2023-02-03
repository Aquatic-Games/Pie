namespace Pie;

/// <summary>
/// Describes when the depth test should pass.
/// </summary>
public enum DepthComparison
{
    /// <summary>
    /// The depth test never passes.
    /// </summary>
    Never,
    
    /// <summary>
    /// The depth test passes if the incoming depth value is less than the stored depth value.
    /// </summary>
    Less,
    
    /// <summary>
    /// The depth test passes if the incoming depth value is equal to the stored depth value.
    /// </summary>
    Equal,
    
    /// <summary>
    /// The depth test passes if the incoming depth value is less than or equal to the stored depth value.
    /// </summary>
    LessEqual,
    
    /// <summary>
    /// The depth test passes if the incoming depth value is greater than the stored depth value.
    /// </summary>
    Greater,
    
    /// <summary>
    /// The depth test passes if the incoming depth value is not equal to the stored depth value.
    /// </summary>
    NotEqual,
    
    /// <summary>
    /// The depth test passes if the incoming depth value is greater than or equal to the stored depth value.
    /// </summary>
    GreaterEqual,
    
    /// <summary>
    /// The depth test will always pass.
    /// </summary>
    Always
}