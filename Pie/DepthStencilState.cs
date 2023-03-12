using System;

namespace Pie;

/// <summary>
/// Used to tell the graphics device how to handle depth information.
/// </summary>
public abstract class DepthStencilState : IDisposable, IEquatable<DepthStencilState>
{
    /// <summary>
    /// Will return <see langword="true" /> when this <see cref="DepthStencilState"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Get the <see cref="DepthStencilStateDescription"/> for this <see cref="DepthStencilState"/>.
    /// </summary>
    public abstract DepthStencilStateDescription Description { get; }

    /// <summary>
    /// Dispose of this <see cref="DepthStencilState"/>.
    /// </summary>
    public abstract void Dispose();

    public bool Equals(DepthStencilState other)
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
        return Equals((DepthStencilState) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsDisposed, Description);
    }

    public static bool operator ==(DepthStencilState left, DepthStencilState right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DepthStencilState left, DepthStencilState right)
    {
        return !Equals(left, right);
    }
}