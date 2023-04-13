using System;

namespace Pie;

/// <summary>
/// Describes a <see cref="RasterizerState"/>.
/// </summary>
public struct RasterizerStateDescription : IEquatable<RasterizerStateDescription>
{
    /// <summary>
    /// No culling whatsoever, with a solid <see cref="Pie.FillMode"/>, and with the scissor test disabled.
    /// </summary>
    public static readonly RasterizerStateDescription CullNone =
        new RasterizerStateDescription(CullFace.None, CullDirection.CounterClockwise, FillMode.Solid, false);

    /// <summary>
    /// Cull back faces in the clockwise direction, with a solid <see cref="Pie.FillMode"/>, and with the scissor test disabled.
    /// </summary>
    public static readonly RasterizerStateDescription CullClockwise =
        new RasterizerStateDescription(CullFace.Back, CullDirection.Clockwise, FillMode.Solid, false);

    /// <summary>
    /// Cull back faces in the counter-clockwise direction, with a solid <see cref="Pie.FillMode"/>, and with the scissor test disabled.
    /// </summary>
    public static readonly RasterizerStateDescription CullCounterClockwise =
        new RasterizerStateDescription(CullFace.Back, CullDirection.CounterClockwise, FillMode.Solid, false);
    
    /// <summary>
    /// The face to cull.
    /// </summary>
    public CullFace CullFace;
    
    /// <summary>
    /// The winding order of the vertices. This sets the front face's direction.
    /// </summary>
    public CullDirection CullDirection;

    /// <summary>
    /// The <see cref="Pie.FillMode"/>.
    /// </summary>
    public FillMode FillMode;

    /// <summary>
    /// Whether or not the scissor test is enabled.
    /// </summary>
    public bool ScissorTest;
    
    /// <summary>
    /// Create a new <see cref="RasterizerStateDescription"/> with the given parameters.
    /// </summary>
    /// <param name="cullFace">The face to cull.</param>
    /// <param name="cullDirection">The winding order of the vertices. This sets the front face's direction.</param>
    /// <param name="fillMode">The <see cref="Pie.FillMode"/>.</param>
    /// <param name="scissorTest">Whether or not the scissor test is enabled.</param>
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