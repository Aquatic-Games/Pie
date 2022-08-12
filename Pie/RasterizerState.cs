namespace Pie;

public abstract class RasterizerState
{
    /// <summary>
    /// Get or set the face that will be culled on next draw. Set to <see cref="CullFace.None"/> to disable face culling.
    /// </summary>
    public abstract CullFace CullFace { get; set; }
    
    /// <summary>
    /// Get or set the face cull direction. This will determine which is the front face and which is the back face.
    /// </summary>
    public abstract CullDirection CullDirection { get; set; }
    
    /// <summary>
    /// Get or set the fill mode that will be used on next draw.
    /// </summary>
    public abstract FillMode FillMode { get; set; }
    
    /// <summary>
    /// Enable or disable the depth mask.
    /// </summary>
    public abstract bool DepthMask { get; set; }
    
    /// <summary>
    /// Enable or disable the scissor test.
    /// </summary>
    public abstract bool EnableScissor { get; set; }

    /// <summary>
    /// Create a new rasterizer state with the default parameters, typical for 3D.
    /// </summary>
    /// <param name="face">The face that will be culled on next draw. Set to <see cref="CullFace.None"/> to disable face culling.</param>
    /// <param name="direction">The face cull direction. This will determine which is the front face and which is the back face.</param>
    /// <param name="fillMode">The fill mode that will be used on next draw.</param>
    /// <param name="depthMask">Whether or not the depth mask will be enabled.</param>
    /// <param name="enableScissor">Whether or not the scissor test will be enabled.</param>
    public RasterizerState(CullFace face = CullFace.Back, CullDirection direction = CullDirection.Clockwise,
        FillMode fillMode = FillMode.Normal, bool depthMask = true, bool enableScissor = false) { }
}