namespace Pie;

/// <summary>
/// Used in <see cref="DepthStencilStateDescription"/> to describe actions that occur for a given stencil face.
/// </summary>
public struct StencilFace
{
    /// <summary>
    /// The operation to perform when the stencil test fails.
    /// </summary>
    public StencilOp StencilFailOp;
    
    /// <summary>
    /// The operation to perform when the depth test fails.
    /// </summary>
    public StencilOp DepthFailOp;
    
    /// <summary>
    /// The operation to perform when both the depth and stencil test pass.
    /// </summary>
    public StencilOp DepthStencilPassOp;
    
    /// <summary>
    /// The function to use when comparing new stencil data to existing stencil data.
    /// </summary>
    public ComparisonFunc StencilFunc;

    /// <summary>
    /// Create a new <see cref="StencilFace"/>.
    /// </summary>
    /// <param name="stencilFailOp">The operation to perform when the stencil test fails.</param>
    /// <param name="depthFailOp">The operation to perform when the depth test fails.</param>
    /// <param name="depthStencilPassOp">The operation to perform when both the depth and stencil test pass.</param>
    /// <param name="stencilFunc">The function to use when comparing new stencil data to existing stencil data.</param>
    public StencilFace(StencilOp stencilFailOp, StencilOp depthFailOp, StencilOp depthStencilPassOp, ComparisonFunc stencilFunc)
    {
        StencilFailOp = stencilFailOp;
        DepthFailOp = depthFailOp;
        DepthStencilPassOp = depthStencilPassOp;
        StencilFunc = stencilFunc;
    }
}