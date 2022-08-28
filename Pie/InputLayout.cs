using System;

namespace Pie;

public abstract class InputLayout : IDisposable, IEquatable<InputLayout>
{
    /// <summary>
    /// Will return <see langword="true" /> when this input layout has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public abstract InputLayoutDescription[] Descriptions { get; }

    public abstract uint Stride { get; }
    
    public abstract void Dispose();

    public bool Equals(InputLayout other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return IsDisposed == other.IsDisposed && Descriptions.Equals(other.Descriptions) && Stride == other.Stride;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((InputLayout) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsDisposed, Descriptions, Stride);
    }

    public static bool operator ==(InputLayout left, InputLayout right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(InputLayout left, InputLayout right)
    {
        return !Equals(left, right);
    }
}