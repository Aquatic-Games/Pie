using System;

namespace Pie;

public abstract class InputLayout : IDisposable, IEquatable<InputLayout>
{
    /// <summary>
    /// Will return <see langword="true" /> when this input layout has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public abstract InputLayoutDescription[] Descriptions { get; }
    
    public abstract void Dispose();

    public bool Equals(InputLayout other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Descriptions, other.Descriptions);
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
        return (Descriptions != null ? Descriptions.GetHashCode() : 0);
    }
}