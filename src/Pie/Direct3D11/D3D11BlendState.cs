using System;
using Vortice.Direct3D11;

namespace Pie.Direct3D11;

internal sealed class D3D11BlendState : BlendState
{
    public override bool IsDisposed { get; protected set; }

    public ID3D11BlendState State;
    
    public D3D11BlendState(ID3D11Device device, BlendStateDescription description)
    {
        Description = description;

        BlendDescription desc = new BlendDescription();
        desc.IndependentBlendEnable = false;
        desc.AlphaToCoverageEnable = false;
        desc.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            BlendEnable = description.Enabled,
            SourceBlend = GetBlendFromBlendType(description.Source),
            DestinationBlend = GetBlendFromBlendType(description.Destination),
            BlendOperation = GetOpFromOp(description.BlendOperation),
            SourceBlendAlpha = GetBlendFromBlendType(description.SourceAlpha),
            DestinationBlendAlpha = GetBlendFromBlendType(description.DestinationAlpha),
            BlendOperationAlpha = GetOpFromOp(description.AlphaBlendOperation),
            // TODO: This should be a safe operation, as the two enums are compatible. But it's going to be better to not cast, so that should probably be done at some point.
            RenderTargetWriteMask = (ColorWriteEnable) description.ColorWriteMask
        };

        State = device.CreateBlendState(desc);
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

    private static Vortice.Direct3D11.BlendOperation GetOpFromOp(BlendOperation operation)
    {
        return operation switch
        {
            BlendOperation.Add => Vortice.Direct3D11.BlendOperation.Add,
            BlendOperation.Subtract => Vortice.Direct3D11.BlendOperation.Subtract,
            BlendOperation.ReverseSubtract => Vortice.Direct3D11.BlendOperation.ReverseSubtract,
            BlendOperation.Min => Vortice.Direct3D11.BlendOperation.Min,
            BlendOperation.Max => Vortice.Direct3D11.BlendOperation.Max,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
}