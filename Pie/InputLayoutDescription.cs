using System;

namespace Pie;

/// <summary>
/// Describes how an <see cref="InputLayout"/> should behave.
/// </summary>
public struct InputLayoutDescription : IEquatable<InputLayoutDescription>
{
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
    /// <param name="format">The format of this attribute.</param>
    /// <param name="offset">The offset, in bytes, of this attribute.</param>
    /// <param name="slot">The vertex buffer slot of this attribute.</param>
    /// <param name="inputType">The input type of this attribute.</param>
    public InputLayoutDescription(Format format, uint offset, uint slot, InputType inputType)
    {
        Format = format;
        Offset = offset;
        Slot = slot;
        InputType = inputType;
    }

    public bool Equals(InputLayoutDescription other)
    {
        return Format == other.Format && Offset == other.Offset && Slot == other.Slot && InputType == other.InputType;
    }

    public override bool Equals(object obj)
    {
        return obj is InputLayoutDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) Format, Offset, Slot, (int) InputType);
    }

    public static bool operator ==(InputLayoutDescription left, InputLayoutDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InputLayoutDescription left, InputLayoutDescription right)
    {
        return !left.Equals(right);
    }
}