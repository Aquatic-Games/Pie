using System;

namespace Pie;

/// <summary>
/// Used to tell the graphics device how to raster triangles.
/// </summary>
public abstract class RasterizerState : IDisposable, IEquatable<RasterizerState>
{
    /// <summary>
    /// Will return <see langword="true" /> when this rasterizer state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The <see cref="RasterizerStateDescription"/> of this <see cref="RasterizerState"/>.
    /// </summary>
    public abstract RasterizerStateDescription Description { get; }

    /// <summary>
    /// Dispose of this <see cref="RasterizerState"/>.
    /// </summary>
    public abstract void Dispose();

    public bool Equals(RasterizerState other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return IsDisposed == other.IsDisposed && Description.Equals(other.Description);
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
        return HashCode.Combine(IsDisposed, Description);
    }

    public static bool operator ==(RasterizerState left, RasterizerState right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RasterizerState left, RasterizerState right)
    {
        return !Equals(left, right);
    }
}