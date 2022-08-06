using System.Numerics;

namespace Pie.Utils;

public struct VertexPositionColor
{
    public Vector3 Position;
    public Vector4 Color;

    public VertexPositionColor(Vector3 position, Vector4 color)
    {
        Position = position;
        Color = color;
    }
    
    public const uint SizeInBytes = 28;
}