using System;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11RasterizerState : RasterizerState
{
    public ID3D11RasterizerState State;
    
    public override bool IsDisposed { get; protected set; }
    public override CullFace CullFace { get; }
    public override CullDirection CullDirection { get; }
    public override FillMode FillMode { get; }
    public override bool EnableScissor { get; }
    
    public D3D11RasterizerState(CullFace face, CullDirection direction, FillMode fillMode, bool enableScissor)
    {
        CullFace = face;
        CullDirection = direction;
        FillMode = fillMode;
        EnableScissor = enableScissor;

        CullMode cullMode = face switch
        {
            CullFace.None => CullMode.None,
            CullFace.Front => CullMode.Front,
            CullFace.Back => CullMode.Back,
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
        };

        Vortice.Direct3D11.FillMode fm = fillMode switch
        {
            FillMode.Solid => Vortice.Direct3D11.FillMode.Solid,
            FillMode.Wireframe => Vortice.Direct3D11.FillMode.Wireframe,
            _ => throw new ArgumentOutOfRangeException(nameof(fillMode), fillMode, null)
        };

        RasterizerDescription description = new RasterizerDescription()
        {
            CullMode = cullMode,
            FrontCounterClockwise = direction == CullDirection.CounterClockwise,
            FillMode = fm,
            ScissorEnable = enableScissor
        };

        State = Device.CreateRasterizerState(description);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }
}