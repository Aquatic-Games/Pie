using System;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11RasterizerState : RasterizerState
{
    public ID3D11RasterizerState State;
    
    public override bool IsDisposed { get; protected set; }

    public override RasterizerStateDescription Description { get; }

    public D3D11RasterizerState(RasterizerStateDescription description)
    {
        Description = description;

        CullMode cullMode = description.CullFace switch
        {
            CullFace.None => CullMode.None,
            CullFace.Front => CullMode.Front,
            CullFace.Back => CullMode.Back,
            _ => throw new ArgumentOutOfRangeException()
        };

        Vortice.Direct3D11.FillMode fm = description.FillMode switch
        {
            FillMode.Solid => Vortice.Direct3D11.FillMode.Solid,
            FillMode.Wireframe => Vortice.Direct3D11.FillMode.Wireframe,
            _ => throw new ArgumentOutOfRangeException()
        };

        RasterizerDescription desc = new RasterizerDescription()
        {
            CullMode = cullMode,
            FrontCounterClockwise = description.CullDirection == CullDirection.CounterClockwise,
            FillMode = fm,
            ScissorEnable = description.ScissorTest
        };

        State = Device.CreateRasterizerState(desc);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }
}