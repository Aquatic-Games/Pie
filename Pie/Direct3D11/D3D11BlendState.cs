using System;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11BlendState : BlendState
{
    public override bool IsDisposed { get; protected set; }

    public ID3D11BlendState State;
    
    public D3D11BlendState(BlendStateDescription description)
    {
        Description = description;

        Vortice.Direct3D11.BlendDescription desc =
            new Vortice.Direct3D11.BlendDescription(GetBlendFromBlendType(description.Source),
                GetBlendFromBlendType(description.Destination));

        State = Device.CreateBlendState(desc);
    }

    public override BlendStateDescription Description { get; }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }

    private static Blend GetBlendFromBlendType(BlendType type)
    {
        return type switch
        {
            BlendType.Zero => Blend.Zero,
            BlendType.One => Blend.One,
            BlendType.SrcColor => Blend.SourceColor,
            BlendType.OneMinusSrcColor => Blend.InverseSourceColor,
            BlendType.DestColor => Blend.DestinationColor,
            BlendType.OneMinusDestColor => Blend.InverseDestinationColor,
            BlendType.SrcAlpha => Blend.SourceAlpha,
            BlendType.OneMinusSrcAlpha => Blend.InverseSourceAlpha,
            BlendType.DestAlpha => Blend.DestinationAlpha,
            BlendType.OneMinusDestAlpha => Blend.InverseDestinationAlpha,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}