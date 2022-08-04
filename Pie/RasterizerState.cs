namespace Pie;

public abstract class RasterizerState
{
    public abstract CullFace CullFace { get; set; }
    
    public abstract CullDirection CullDirection { get; set; }
    
    public abstract FillMode FillMode { get; set; }
    
    public abstract bool DepthMask { get; set; }
    
    public abstract bool EnableScissor { get; set; }

    public RasterizerState(CullFace face = CullFace.Back, CullDirection direction = CullDirection.Clockwise,
        FillMode fillMode = FillMode.Normal, bool depthMask = true, bool enableScissor = false) { }
}