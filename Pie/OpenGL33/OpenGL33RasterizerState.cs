using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33RasterizerState : RasterizerState
{
    private CullFace _cullFace;
    private CullDirection _direction;
    private FillMode _fillMode;
    private bool _depthMask;
    private bool _scissorEnabled;
    
    public override CullFace CullFace
    {
        get => CullFace.None;
        set
        {
            CullFace face = _cullFace;
            _cullFace = value;

            if (value == CullFace.None)
            {
                Gl.Disable(EnableCap.CullFace);
                return;
            }

            if (face == CullFace.None)
                Gl.Enable(EnableCap.CullFace);
            
            Gl.CullFace(value switch
            {
                CullFace.Front => CullFaceMode.Front,
                CullFace.Back => CullFaceMode.Back,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            });
        }
    }

    public override CullDirection CullDirection
    {
        get => _direction;
        set
        {
            _direction = value;
            Gl.FrontFace(value switch
            {
                CullDirection.Clockwise => FrontFaceDirection.CW,
                CullDirection.CounterClockwise => FrontFaceDirection.Ccw,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            });
        }
    }

    public override FillMode FillMode
    {
        get => _fillMode;
        set
        {
            _fillMode = value;
            Gl.PolygonMode(MaterialFace.FrontAndBack, value == FillMode.Normal ? PolygonMode.Fill : PolygonMode.Line);
        }
    }

    public override bool DepthMask
    {
        get => _depthMask;
        set
        {
            _depthMask = value;
            Gl.DepthMask(value);
        }
    }

    public override bool EnableScissor
    {
        get => _scissorEnabled;
        set
        {
            _scissorEnabled = value;
            if (value)
                Gl.Enable(EnableCap.ScissorTest);
            else
                Gl.Disable(EnableCap.ScissorTest);
        }
    }

    public OpenGL33RasterizerState(CullFace face = CullFace.Back, CullDirection direction = CullDirection.Clockwise,
        FillMode fillMode = FillMode.Normal, bool depthMask = true, bool enableScissor = false) : base(face, direction,
        fillMode, depthMask, enableScissor)
    {
        CullFace = face;
        CullDirection = direction;
        FillMode = fillMode;
        DepthMask = depthMask;
        EnableScissor = enableScissor;
    }
}