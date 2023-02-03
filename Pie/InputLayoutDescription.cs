using System;

namespace Pie;

/// <summary>
/// Describes how an <see cref="InputLayout"/> should behave.
/// </summary>
public struct InputLayoutDescription : IEquatable<InputLayoutDescription>
{
    /// <summary>
    /// The name of this attribute.
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The format of this attribute.
    /// </summary>
    public readonly Format Format;

    /// <summary>
    /// The offset, in bytes, of this attribute.
    /// </summary>
    public readonly uint Offset;

    /// <summary>
    /// The vertex buffer slot of this attribute.
    /// </summary>
    public uint Slot;

    /// <summary>
    /// The input type of this attribute.
    /// </summary>
    public InputType InputType;

    /// <summary>
    /// Create a new input layout description for use with an <see cref="InputLayout"/>.
    /// </summary>
    /// <param name="name">The name of this attribute.</param>
    /// <param name="format">The format of this attribute.</param>
    /// <param name="offset">The offset, in bytes, of this attribute.</param>
    /// <param name="slot">The vertex buffer slot of this attribute.</param>
    /// <param name="inputType">The input type of this attribute.</param>
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