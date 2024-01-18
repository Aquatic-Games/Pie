using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlDepthStencilState : DepthStencilState
{
    private DepthStencilStateDescription _description;
    private DepthFunction _depthFunction;

    private OpenTK.Graphics.OpenGL4.StencilOp _frontStencilPass;
    private OpenTK.Graphics.OpenGL4.StencilOp _frontStencilFail;
    private OpenTK.Graphics.OpenGL4.StencilOp _frontDepthFail;
    private StencilFunction _frontFunc;
    
    private OpenTK.Graphics.OpenGL4.StencilOp _backStencilPass;
    private OpenTK.Graphics.OpenGL4.StencilOp _backStencilFail;
    private OpenTK.Graphics.OpenGL4.StencilOp _backDepthFail;
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
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(_description.DepthMask);
            GL.DepthFunc(_depthFunction);
        }
        else
            GL.Disable(EnableCap.DepthTest);

        if (_description.StencilEnabled)
        {
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOpSeparate(OpenTK.Graphics.OpenGL4.StencilFace.Front, _frontStencilFail, _frontDepthFail, _frontStencilPass);
            GL.StencilOpSeparate(OpenTK.Graphics.OpenGL4.StencilFace.Back, _backStencilFail, _backDepthFail, _backStencilPass);
            GL.StencilMask(_description.StencilWriteMask);
            GL.StencilFuncSeparate(OpenTK.Graphics.OpenGL4.StencilFace.Front, _frontFunc, stencilRef, _description.StencilReadMask);
            GL.StencilFuncSeparate(OpenTK.Graphics.OpenGL4.StencilFace.Back, _backFunc, stencilRef, _description.StencilReadMask);
        }
        else
            GL.Disable(EnableCap.StencilTest);
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

    private OpenTK.Graphics.OpenGL4.StencilOp StencilOpToOp(StencilOp op)
    {
        return op switch
        {
            StencilOp.Keep => OpenTK.Graphics.OpenGL4.StencilOp.Keep,
            StencilOp.Zero => OpenTK.Graphics.OpenGL4.StencilOp.Zero,
            StencilOp.Replace => OpenTK.Graphics.OpenGL4.StencilOp.Replace,
            StencilOp.Increment => OpenTK.Graphics.OpenGL4.StencilOp.Incr,
            StencilOp.IncrementWrap => OpenTK.Graphics.OpenGL4.StencilOp.IncrWrap,
            StencilOp.Decrement => OpenTK.Graphics.OpenGL4.StencilOp.Decr,
            StencilOp.DecrementWrap => OpenTK.Graphics.OpenGL4.StencilOp.DecrWrap,
            StencilOp.Invert => OpenTK.Graphics.OpenGL4.StencilOp.Invert,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private OpenTK.Graphics.OpenGL4.All FuncToEnum(ComparisonFunc func)
    {
        return func switch
        {
            ComparisonFunc.Never => All.Never,
            ComparisonFunc.Less => All.Less,
            ComparisonFunc.Equal => All.Equal,
            ComparisonFunc.LessEqual => All.Lequal,
            ComparisonFunc.Greater => All.Greater,
            ComparisonFunc.NotEqual => All.Notequal,
            ComparisonFunc.GreaterEqual => All.Gequal,
            ComparisonFunc.Always => All.Always,
            _ => throw new ArgumentOutOfRangeException(nameof(func), func, null)
        };
    }
}