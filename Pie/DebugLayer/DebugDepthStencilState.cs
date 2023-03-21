namespace Pie.DebugLayer;

internal sealed class DebugDepthStencilState : DepthStencilState
{
    public DepthStencilState DepthStencilState;

    public override bool IsDisposed { get; protected set; }

    public override DepthStencilStateDescription Description => DepthStencilState.Description;

    public DebugDepthStencilState(GraphicsDevice device, DepthStencilStateDescription description)
    {
        PieLog.Log(LogType.Debug, $@"Depth stencil state info:
    DepthEnabled: {description.DepthEnabled}
    DepthComparison: {description.DepthComparison}
    DepthMask: {description.DepthMask}
    StencilEnabled: {description.StencilEnabled}
    StencilFrontFace: {description.StencilFrontFace}
    StencilBackSpace: {description.StencilBackFace}
    StencilReadMask: {description.StencilReadMask}
    StencilWriteMask: {description.StencilWriteMask}");
        
        DepthStencilState = device.CreateDepthStencilState(description);
    }
    
    public override void Dispose()
    {
        DepthStencilState.Dispose();
        IsDisposed = DepthStencilState.IsDisposed;
        PieLog.Log(LogType.Debug, "Depth stencil state disposed.");
    }
}