using System;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11DepthState : DepthState
{
    public ID3D11DepthStencilState State;
    
    public D3D11DepthState(DepthStateDescription description)
    {
        Description = description;

        DepthStencilDescription desc = new DepthStencilDescription(description.DepthEnabled,
            description.DepthMask ? DepthWriteMask.All : DepthWriteMask.Zero, description.DepthComparison switch
            {
                DepthComparison.Never => ComparisonFunction.Never,
                DepthComparison.Less => ComparisonFunction.Less,
                DepthComparison.Equal => ComparisonFunction.Equal,
                DepthComparison.LessEqual => ComparisonFunction.LessEqual,
                DepthComparison.Greater => ComparisonFunction.Greater,
                DepthComparison.NotEqual => ComparisonFunction.NotEqual,
                DepthComparison.GreaterEqual => ComparisonFunction.GreaterEqual,
                DepthComparison.Always => ComparisonFunction.Always,
                _ => throw new ArgumentOutOfRangeException()
            });

        State = Device.CreateDepthStencilState(desc);
    }

    public override bool IsDisposed { get; protected set; }
    
    public override DepthStateDescription Description { get; }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }
}