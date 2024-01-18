using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlRasterizerState : RasterizerState
{
    private bool _cullFaceEnabled;
    private CullFaceMode _cullFaceMode;
    private FrontFaceDirection _frontFace;
    private PolygonMode _mode;
    private bool _scissor;
    
    public GlRasterizerState(RasterizerStateDescription description)
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
            CullDirection.Clockwise => FrontFaceDirection.Cw,
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
            GL.Disable(EnableCap.CullFace);
        else
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(_cullFaceMode);
        }
        
        GL.FrontFace(_frontFace);

        if (!GlGraphicsDevice.IsES)
            GL.PolygonMode(MaterialFace.FrontAndBack, _mode);
        
        if (_scissor)
            GL.Enable(EnableCap.ScissorTest);
        else
            GL.Disable(EnableCap.ScissorTest);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to dispose.
    }
}