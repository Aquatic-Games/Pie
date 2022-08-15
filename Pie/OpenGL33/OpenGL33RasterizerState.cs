using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33RasterizerState : RasterizerState
{
    public override bool IsDisposed { get; protected set; }
    public override CullFace CullFace { get; }
    public override CullDirection CullDirection { get; }
    public override FillMode FillMode { get; }
    public override bool EnableScissor { get; }

    public OpenGL33RasterizerState(CullFace face, CullDirection direction, FillMode fillMode, bool enableScissor)
    {
        CullFace = face;
        CullDirection = direction;
        FillMode = fillMode;
        EnableScissor = enableScissor;
    }
    
    public void Set()
    {
        if (CullFace == CullFace.None)
            Gl.Disable(EnableCap.CullFace);
        else
        {
            Gl.Enable(EnableCap.CullFace);
            CullFaceMode cullMode = CullFace switch
            {
                CullFace.Front => CullFaceMode.Front,
                CullFace.Back => CullFaceMode.Back,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Gl.CullFace(cullMode);
        }

        FrontFaceDirection dir = CullDirection switch
        {
            CullDirection.Clockwise => FrontFaceDirection.CW,
            CullDirection.CounterClockwise => FrontFaceDirection.Ccw,
            _ => throw new ArgumentOutOfRangeException()
        };
        Gl.FrontFace(dir);

        Gl.PolygonMode(MaterialFace.FrontAndBack, FillMode == FillMode.Solid ? PolygonMode.Fill : PolygonMode.Line);
        
        if (EnableScissor)
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