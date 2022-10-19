using System;
using Silk.NET.OpenGLES;
using static Pie.OpenGLES20.OpenGLES20GraphicsDevice;

namespace Pie.OpenGLES20;

internal sealed class OpenGLES20RasterizerState : RasterizerState
{
    private bool _cullFaceEnabled;
    private CullFaceMode _cullFaceMode;
    private FrontFaceDirection _frontFace;
    private bool _scissor;
    
    public OpenGLES20RasterizerState(RasterizerStateDescription description)
    {
        Description = description;

        if (description.CullFace == CullFace.None)
            _cullFaceEnabled = false;
        else
        {
            _cullFaceEnabled = true;
            _cullFaceMode = description.CullFace switch
            {
                CullFace.Front => CullFaceMode.Front,
                CullFace.Back => CullFaceMode.Back,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        _frontFace = description.CullDirection switch
        {
            CullDirection.Clockwise => FrontFaceDirection.CW,
            CullDirection.CounterClockwise => FrontFaceDirection.Ccw,
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (description.FillMode)
        {
            case FillMode.Solid:
                break;
            case FillMode.Wireframe:
                Logging.Log(LogType.Warning, "OpenGL ES only supports GL_FILL, however you have chosen wireframe, which will be ignored.");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _scissor = description.ScissorTest;
    }
    
    public override bool IsDisposed { get; protected set; }

    public override RasterizerStateDescription Description { get; }
    
    public void Set()
    {
        if (!_cullFaceEnabled)
            Gl.Disable(EnableCap.CullFace);
        else
        {
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(_cullFaceMode);
        }
        
        Gl.FrontFace(_frontFace);

        if (_scissor)
            Gl.Enable(EnableCap.ScissorTest);
        else
            Gl.Disable(EnableCap.ScissorTest);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to dispose.
    }
}