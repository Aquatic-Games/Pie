using System;

namespace Pie;

public abstract class RasterizerState : IDisposable, IEquatable<RasterizerState>
{
    /// <summary>
    /// Will return <see langword="true" /> when this rasterizer state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Get or set the face that will be culled on next draw. Set to <see cref="CullFace.None"/> to disable face culling.
    /// </summary>
    public abstract CullFace CullFace { get; }
    
    /// <summary>
    /// Get or set the face cull direction. This will determine which is the front face and which is the back face.
    /// </summary>
    public abstract CullDirection CullDirection { get; }
    
    /// <summary>
    /// Get or set the fill mode that will be used on next draw.
    /// </summary>
    public abstract FillMode FillMode { get; }

    /// <summary>
    /// Enable or disable the scissor test.
    /// </summary>
    public abstract bool EnableScissor { get; }

    public abstract void Dispose();

    public bool Equals(RasterizerState other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return CullFace == other.CullFace && CullDirection == other.CullDirection && FillMode == other.FillMode && EnableScissor == other.EnableScissor;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((RasterizerState) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) CullFace, (int) CullDirection, (int) FillMode, EnableScissor);
    }
}