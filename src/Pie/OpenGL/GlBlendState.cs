using System;
using Silk.NET.OpenGL;

namespace Pie.OpenGL;

internal sealed class GlBlendState : BlendState
{
    private BlendStateDescription _description;
    
    private BlendingFactor _src;
    private BlendingFactor _dst;
    private BlendingFactor _srcAlpha;
    private BlendingFactor _dstAlpha;
    private BlendEquationModeEXT _rgbEq;
    private BlendEquationModeEXT _alphaEq;

    private bool _red, _green, _blue, _alpha;
    
    public GlBlendState(BlendStateDescription description)
    {
        _description = description;
        
        _src = GetBlendingFactorFromBlendType(description.Source);
        _dst = GetBlendingFactorFromBlendType(description.Destination);

        _srcAlpha = GetBlendingFactorFromBlendType(description.SourceAlpha);
        _dstAlpha = GetBlendingFactorFromBlendType(description.DestinationAlpha);

        _rgbEq = GetEquationFromOp(description.BlendOperation);
        _alphaEq = GetEquationFromOp(description.AlphaBlendOperation);

        _red = (description.ColorWriteMask & ColorWriteMask.Red) == ColorWriteMask.Red;
        _green = (description.ColorWriteMask & ColorWriteMask.Green) == ColorWriteMask.Green;
        _blue = (description.ColorWriteMask & ColorWriteMask.Blue) == ColorWriteMask.Blue;
        _alpha = (description.ColorWriteMask & ColorWriteMask.Alpha) == ColorWriteMask.Alpha;
    }
    
    public override bool IsDisposed { get; protected set; }

    public override BlendStateDescription Description => _description;

    public void Set(GL gl)
    {
        gl.ColorMask(_red, _green, _blue, _alpha);
        
        if (!_description.Enabled)
        {
            gl.Disable(EnableCap.Blend);
            return;
        }
        
        gl.Enable(EnableCap.Blend);
        gl.BlendFuncSeparate(_src, _dst, _srcAlpha, _dstAlpha);
        gl.BlendEquationSeparate(_rgbEq, _alphaEq);
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

    private static BlendEquationModeEXT GetEquationFromOp(BlendOperation operation)
    {
        return operation switch
        {
            BlendOperation.Add => BlendEquationModeEXT.FuncAdd,
            BlendOperation.Subtract => BlendEquationModeEXT.FuncSubtract,
            BlendOperation.ReverseSubtract => BlendEquationModeEXT.FuncReverseSubtract,
            BlendOperation.Min => BlendEquationModeEXT.Min,
            BlendOperation.Max => BlendEquationModeEXT.Max,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
}