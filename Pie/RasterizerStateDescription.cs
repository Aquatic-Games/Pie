using System;

namespace Pie;

public struct RasterizerStateDescription : IEquatable<RasterizerStateDescription>
{
    public static readonly RasterizerStateDescription CullNone =
        new RasterizerStateDescription(CullFace.None, CullDirection.CounterClockwise, FillMode.Solid, false);

    public static readonly RasterizerStateDescription CullClockwise =
        new RasterizerStateDescription(CullFace.Back, CullDirection.Clockwise, FillMode.Solid, false);

    public static readonly RasterizerStateDescription CullCounterClockwise =
        new RasterizerStateDescription(CullFace.Back, CullDirection.CounterClockwise, FillMode.Solid, false);
    
    public CullFace CullFace;
    
    public CullDirection CullDirection;

    public FillMode FillMode;

    public bool ScissorTest;
    
    public RasterizerStateDescription(CullFace cullFace, CullDirection cullDirection, FillMode fillMode, bool scissorTest)
    {
        CullFace = cullFace;
        CullDirection = cullDirection;
        FillMode = fillMode;
        ScissorTest = scissorTest;
    }

    public bool Equals(RasterizerStateDescription other)
    {
        return CullFace == other.CullFace && CullDirection == other.CullDirection && FillMode == other.FillMode && ScissorTest == other.ScissorTest;
    }

    public override bool Equals(object obj)
    {
        return obj is RasterizerStateDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) CullFace, (int) CullDirection, (int) FillMode, ScissorTest);
    }

    public static bool operator ==(RasterizerStateDescription left, RasterizerStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RasterizerStateDescription left, RasterizerStateDescription right)
    {
        return !left.Equals(right);
    }
}