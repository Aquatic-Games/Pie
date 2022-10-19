using System;
using Silk.NET.OpenGLES;
using static Pie.OpenGLES20.OpenGLES20GraphicsDevice;

namespace Pie.OpenGLES20;

internal sealed class OpenGLES20DepthState : DepthState
{
    private bool _depthEnabled;
    private bool _depthMask;
    private DepthFunction _depthFunction;
    
    public OpenGLES20DepthState(DepthStateDescription description)
    {
        Description = description;

        _depthEnabled = description.DepthEnabled;
        _depthMask = description.DepthMask;
        _depthFunction = description.DepthComparison switch
        {
            DepthComparison.Never => DepthFunction.Never,
            DepthComparison.Less => DepthFunction.Less,
            DepthComparison.Equal => DepthFunction.Equal,
            DepthComparison.LessEqual => DepthFunction.Lequal,
            DepthComparison.Greater => DepthFunction.Greater,
            DepthComparison.NotEqual => DepthFunction.Notequal,
            DepthComparison.GreaterEqual => DepthFunction.Gequal,
            DepthComparison.Always => DepthFunction.Always,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Set()
    {
        if (_depthEnabled)
            Gl.Enable(EnableCap.DepthTest);
        else
        {
            Gl.Disable(EnableCap.DepthTest);
            return;
        }
        
        Gl.DepthMask(_depthMask);
        Gl.DepthFunc(_depthFunction);
    }

    public override bool IsDisposed { get; protected set; }
    public override DepthStateDescription Description { get; }
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to dispose.
    }
}