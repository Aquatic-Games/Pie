using System;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11BlendState : BlendState
{
    public override bool IsDisposed { get; protected set; }

    public ComPtr<ID3D11BlendState> State;
    
    public D3D11BlendState(BlendStateDescription description)
    {
        Description = description;

        BlendDesc desc = new BlendDesc();
        desc.IndependentBlendEnable = false;
        desc.AlphaToCoverageEnable = false;
        desc.RenderTarget[0] = new RenderTargetBlendDesc()
        {
            BlendEnable = description.Enabled,
            SrcBlend = GetBlendFromBlendType(description.Source),
            DestBlend = GetBlendFromBlendType(description.Destination),
            BlendOp = GetOpFromOp(description.BlendOperation),
            SrcBlendAlpha = GetBlendFromBlendType(description.SourceAlpha),
            DestBlendAlpha = GetBlendFromBlendType(description.DestinationAlpha),
            BlendOpAlpha = GetOpFromOp(description.AlphaBlendOperation),
            RenderTargetWriteMask = (byte) description.ColorWriteMask
        };

        if (!Succeeded(Device.CreateBlendState(&desc, ref State)))
            throw new PieException("Failed to create blend state.");
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
            BlendType.SrcColor => Blend.SrcColor,
            BlendType.OneMinusSrcColor => Blend.InvSrcColor,
            BlendType.DestColor => Blend.DestColor,
            BlendType.OneMinusDestColor => Blend.InvDestColor,
            BlendType.SrcAlpha => Blend.SrcAlpha,
            BlendType.OneMinusSrcAlpha => Blend.InvSrcAlpha,
            BlendType.DestAlpha => Blend.DestAlpha,
            BlendType.OneMinusDestAlpha => Blend.InvDestAlpha,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static BlendOp GetOpFromOp(BlendOperation operation)
    {
        return operation switch
        {
            BlendOperation.Add => BlendOp.Add,
            BlendOperation.Subtract => BlendOp.Subtract,
            BlendOperation.ReverseSubtract => BlendOp.RevSubtract,
            BlendOperation.Min => BlendOp.Min,
            BlendOperation.Max => BlendOp.Max,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
}