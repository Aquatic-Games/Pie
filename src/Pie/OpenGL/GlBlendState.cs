using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlBlendState : BlendState
{
    private BlendStateDescription _description;
    
    private BlendingFactorSrc _src;
    private BlendingFactorDest _dst;
    private BlendingFactorSrc _srcAlpha;
    private BlendingFactorDest _dstAlpha;
    private BlendEquationMode _rgbEq;
    private BlendEquationMode _alphaEq;

    private bool _red, _green, _blue, _alpha;
    
    public GlBlendState(BlendStateDescription description)
    {
        _description = description;
        
        _src = (BlendingFactorSrc) GetBlendingFactorFromBlendType(description.Source);
        _dst = (BlendingFactorDest) GetBlendingFactorFromBlendType(description.Destination);

        _srcAlpha = (BlendingFactorSrc) GetBlendingFactorFromBlendType(description.SourceAlpha);
        _dstAlpha = (BlendingFactorDest) GetBlendingFactorFromBlendType(description.DestinationAlpha);

        _rgbEq = GetEquationFromOp(description.BlendOperation);
        _alphaEq = GetEquationFromOp(description.AlphaBlendOperation);

        _red = (description.ColorWriteMask & ColorWriteMask.Red) == ColorWriteMask.Red;
        _green = (description.ColorWriteMask & ColorWriteMask.Green) == ColorWriteMask.Green;
        _blue = (description.ColorWriteMask & ColorWriteMask.Blue) == ColorWriteMask.Blue;
        _alpha = (description.ColorWriteMask & ColorWriteMask.Alpha) == ColorWriteMask.Alpha;
    }
    
    public override bool IsDisposed { get; protected set; }

    public override BlendStateDescription Description => _description;

    public void Set()
    {
        GL.ColorMask(_red, _green, _blue, _alpha);
        
        if (!_description.Enabled)
        {
            GL.Disable(EnableCap.Blend);
            return;
        }
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFuncSeparate(_src, _dst, _srcAlpha, _dstAlpha);
        GL.BlendEquationSeparate(_rgbEq, _alphaEq);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to dispose of
    }

    private static BlendingFactor GetBlendingFactorFromBlendType(BlendType type)
    {
        return type switch
        {
            BlendType.Zero => BlendingFactor.Zero,
            BlendType.One => BlendingFactor.One,
            BlendType.SrcColor => BlendingFactor.SrcColor,
            BlendType.OneMinusSrcColor => BlendingFactor.OneMinusSrcColor,
            BlendType.DestColor => BlendingFactor.DstColor,
            BlendType.OneMinusDestColor => BlendingFactor.OneMinusDstColor,
            BlendType.SrcAlpha => BlendingFactor.SrcAlpha,
            BlendType.OneMinusSrcAlpha => BlendingFactor.OneMinusSrcAlpha,
            BlendType.DestAlpha => BlendingFactor.DstAlpha,
            BlendType.OneMinusDestAlpha => BlendingFactor.OneMinusDstAlpha,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static BlendEquationMode GetEquationFromOp(BlendOperation operation)
    {
        return operation switch
        {
            BlendOperation.Add => BlendEquationMode.FuncAdd,
            BlendOperation.Subtract => BlendEquationMode.FuncSubtract,
            BlendOperation.ReverseSubtract => BlendEquationMode.FuncReverseSubtract,
            BlendOperation.Min => BlendEquationMode.Min,
            BlendOperation.Max => BlendEquationMode.Max,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
}