using System;

namespace Pie;

public struct InputLayoutDescription : IEquatable<InputLayoutDescription>
{
    /// <summary>
    /// The name of this attribute.
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The format of this attribute. This is also used to determine its size.
    /// </summary>
    public readonly Format Format;

    public readonly uint Offset;

    /// <summary>
    /// The vertex buffer slot of this attribute.
    /// </summary>
    public uint Slot;

    /// <summary>
    /// The input format of data.
    /// </summary>
    public InputType InputType;

    /// <summary>
    /// Create a new input layout description for use with an <see cref="InputLayout"/>.
    /// </summary>
    /// <param name="name">The name of this attribute.</param>
    /// <param name="format">The format of this attribute. This is also used to determine its size.</param>
    public InputLayoutDescription(string name, Format format, uint offset, uint slot, InputType inputType)
    {
        Name = name;
        Format = format;
        Offset = offset;
        Slot = slot;
        InputType = inputType;
    }

    public bool Equals(InputLayoutDescription other)
    {
        return Name == other.Name && Format == other.Format;
    }

    public override bool Equals(object obj)
    {
        return obj is InputLayoutDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, (int) Format);
    }
}