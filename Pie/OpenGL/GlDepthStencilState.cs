using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GlGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GlDepthStencilState : DepthStencilState
{
    private DepthStencilStateDescription _description;
    private DepthFunction _depthFunction;

    private Silk.NET.OpenGL.StencilOp _frontStencilPass;
    private Silk.NET.OpenGL.StencilOp _frontStencilFail;
    private Silk.NET.OpenGL.StencilOp _frontDepthFail;
    private StencilFunction _frontFunc;
    
    private Silk.NET.OpenGL.StencilOp _backStencilPass;
    private Silk.NET.OpenGL.StencilOp _backStencilFail;
    private Silk.NET.OpenGL.StencilOp _backDepthFail;
    private StencilFunction _backFunc;

    public GlDepthStencilState(DepthStencilStateDescription description)
    {
        _description = description;

        _depthFunction = (DepthFunction) FuncToEnum(description.DepthComparison);

        _frontStencilPass = StencilOpToOp(description.StencilFrontFace.DepthStencilPassOp);
        _frontStencilFail = StencilOpToOp(description.StencilFrontFace.StencilFailOp);
        _frontDepthFail = StencilOpToOp(description.StencilFrontFace.DepthFailOp);
        _frontFunc = (StencilFunction) FuncToEnum(description.StencilFrontFace.StencilFunc);
        
        _backStencilPass = StencilOpToOp(description.StencilBackFace.DepthStencilPassOp);
        _backStencilFail = StencilOpToOp(description.StencilBackFace.StencilFailOp);
        _backDepthFail = StencilOpToOp(description.StencilBackFace.DepthFailOp);
        _backFunc = (StencilFunction) FuncToEnum(description.StencilBackFace.StencilFunc);
    }

    public void Set(int stencilRef)
    {
        if (_description.DepthEnabled)
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthMask(_description.DepthMask);
            Gl.DepthFunc(_depthFunction);
        }
        else
            Gl.Disable(EnableCap.DepthTest);

        if (_description.StencilEnabled)
        {
            Gl.Enable(EnableCap.StencilTest);
            Gl.StencilOpSeparate(StencilFaceDirection.Front, _frontStencilFail, _frontDepthFail, _frontStencilPass);
            Gl.StencilOpSeparate(StencilFaceDirection.Back, _backStencilFail, _backDepthFail, _backStencilPass);
            Gl.StencilMask(_description.StencilWriteMask);
            // TODO: Is this right? Is ref meant to be 1? Is this the read mask? aaaaa?
            Gl.StencilFuncSeparate(StencilFaceDirection.Front, _frontFunc, stencilRef, _description.StencilReadMask);
            Gl.StencilFuncSeparate(StencilFaceDirection.Back, _backFunc, stencilRef, _description.StencilReadMask);
        }
        else
            Gl.Disable(EnableCap.StencilTest);
    }

    public override bool IsDisposed { get; protected set; }
    public override DepthStencilStateDescription Description => _description;
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to dispose.
    }

    private Silk.NET.OpenGL.StencilOp StencilOpToOp(StencilOp op)
    {
        return op switch
        {
            StencilOp.Keep => Silk.NET.OpenGL.StencilOp.Keep,
            StencilOp.Zero => Silk.NET.OpenGL.StencilOp.Zero,
            StencilOp.Replace => Silk.NET.OpenGL.StencilOp.Replace,
            StencilOp.Increment => Silk.NET.OpenGL.StencilOp.Incr,
            StencilOp.IncrementWrap => Silk.NET.OpenGL.StencilOp.IncrWrap,
            StencilOp.Decrement => Silk.NET.OpenGL.StencilOp.Decr,
            StencilOp.DecrementWrap => Silk.NET.OpenGL.StencilOp.DecrWrap,
            StencilOp.Invert => Silk.NET.OpenGL.StencilOp.Invert,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private GLEnum FuncToEnum(ComparisonFunc func)
    {
        return func switch
        {
            ComparisonFunc.Never => GLEnum.Never,
            ComparisonFunc.Less => GLEnum.Less,
            ComparisonFunc.Equal => GLEnum.Equal,
            ComparisonFunc.LessEqual => GLEnum.Lequal,
            ComparisonFunc.Greater => GLEnum.Greater,
            ComparisonFunc.NotEqual => GLEnum.Notequal,
            ComparisonFunc.GreaterEqual => GLEnum.Gequal,
            ComparisonFunc.Always => GLEnum.Always,
        };
    }
}