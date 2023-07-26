using System;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11RasterizerState : RasterizerState
{
    public ComPtr<ID3D11RasterizerState> State;
    
    public override bool IsDisposed { get; protected set; }

    public override RasterizerStateDescription Description { get; }

    public D3D11RasterizerState(ComPtr<ID3D11Device> device, RasterizerStateDescription description)
    {
        Description = description;

        CullMode cullMode = description.CullFace switch
        {
            CullFace.None => CullMode.None,
            CullFace.Front => CullMode.Front,
            CullFace.Back => CullMode.Back,
            _ => throw new ArgumentOutOfRangeException()
        };

        Silk.NET.Direct3D11.FillMode fm = description.FillMode switch
        {
            FillMode.Solid => Silk.NET.Direct3D11.FillMode.Solid,
            FillMode.Wireframe => Silk.NET.Direct3D11.FillMode.Wireframe,
            _ => throw new ArgumentOutOfRangeException()
        };

        RasterizerDesc desc = new RasterizerDesc()
        {
            CullMode = cullMode,
            FrontCounterClockwise = description.CullDirection == CullDirection.CounterClockwise,
            FillMode = fm,
            ScissorEnable = description.ScissorTest
        };

        if (!Succeeded(device.CreateRasterizerState(&desc, ref State)))
            throw new PieException("Failed to create rasterizer state.");
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }
}