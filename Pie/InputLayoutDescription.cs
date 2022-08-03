namespace Pie;

public struct InputLayoutDescription
{
    public readonly string Name;
    public readonly AttributeType Type;

    public InputLayoutDescription(string name, AttributeType type)
    {
        Name = name;
        Type = type;
    }
}