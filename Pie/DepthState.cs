using System;

namespace Pie;

public abstract class DepthState : IDisposable, IEquatable<DepthState>
{
    /// <summary>
    /// Will return <see langword="true" /> when this depth state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public abstract DepthStateDescription Description { get; }

    public abstract void Dispose();

    public bool Equals(DepthState other)
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
        return Equals((DepthState) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsDisposed, Description);
    }

    public static bool operator ==(DepthState left, DepthState right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DepthState left, DepthState right)
    {
        return !Equals(left, right);
    }
}