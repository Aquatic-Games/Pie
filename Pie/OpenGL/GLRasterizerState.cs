using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GLRasterizerState : RasterizerState
{
    private bool _cullFaceEnabled;
    private CullFaceMode _cullFaceMode;
    private FrontFaceDirection _frontFace;
    private PolygonMode _mode;
    private bool _scissor;
    
    public GLRasterizerState(RasterizerStateDescription description)
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

        _mode = description.FillMode switch
        {
            FillMode.Solid => PolygonMode.Fill,
            FillMode.Wireframe => PolygonMode.Line,
            _ => throw new ArgumentOutOfRangeException()
        };

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

        Gl.PolygonMode(MaterialFace.FrontAndBack, _mode);
        
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