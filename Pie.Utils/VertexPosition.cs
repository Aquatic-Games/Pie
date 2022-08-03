using System.Numerics;

namespace Pie.Utils;

public struct VertexPosition
{
    public Vector3 Position;

    public VertexPosition(Vector3 position)
    {
        Position = position;
    }

    public const uint SizeInBytes = 12;
}