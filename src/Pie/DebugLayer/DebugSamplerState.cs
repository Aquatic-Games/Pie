using static Pie.Debugging.DebugGraphicsDevice;

namespace Pie.DebugLayer;

internal sealed class DebugSamplerState : SamplerState
{
    public SamplerState SamplerState;
    
    public override bool IsDisposed { get; protected set; }

    public override SamplerStateDescription Description => SamplerState.Description;

    public DebugSamplerState(SamplerStateDescription description)
    {
        PieLog.Log(LogType.Debug, $@"Sampler state info:
    Filter: {description.Filter}
    AddressU: {description.AddressU}
    AddressV: {description.AddressV}
    AddressW: {description.AddressW}
    MaxAnisotropy: {description.MaxAnisotropy}
    MinLOD: {description.MinLOD}
    MaxLOD: {description.MaxLOD}
    BorderColor: {description.BorderColor}");

        SamplerState = Device.CreateSamplerState(description);
    }
    
    public override void Dispose()
    {
        SamplerState.Dispose();
        IsDisposed = SamplerState.IsDisposed;
        PieLog.Log(LogType.Debug, "Sampler state disposed.");
    }
}