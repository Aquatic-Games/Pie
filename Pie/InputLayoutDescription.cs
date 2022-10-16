using System;

namespace Pie;

public struct InputLayoutDescription : IEquatable<InputLayoutDescription>
{
    /// <summary>
    /// The name of this attribute.
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The type of this attribute. This is also used to determine its size.
    /// </summary>
    public readonly AttributeType Type;

    public readonly uint Offset;

    /// <summary>
    /// The vertex buffer slot of this attribute.
    /// </summary>
    public uint Slot;

    /// <summary>
    /// The input type of data.
    /// </summary>
    public InputType InputType;

    /// <summary>
    /// Create a new input layout description for use with an <see cref="InputLayout"/>.
    /// </summary>
    /// <param name="name">The name of this attribute.</param>
    /// <param name="type">The type of this attribute. This is also used to determine its size.</param>
    public InputLayoutDescription(string name, AttributeType type, uint offset, uint slot, InputType inputType)
    {
        Name = name;
        Type = type;
        Offset = offset;
        Slot = slot;
        InputType = inputType;
    }

    public bool Equals(InputLayoutDescription other)
    {
        return Name == other.Name && Type == other.Type;
    }

    public override bool Equals(object obj)
    {
        return obj is InputLayoutDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, (int) Type);
    }
}