using System.Numerics;

namespace Pie.Utils;

public struct VertexPositionTexture
{
    public Vector3 Position;
    public Vector2 TexCoords;

    public VertexPositionTexture(Vector3 position, Vector2 texCoords)
    {
        Position = position;
        TexCoords = texCoords;
    }

    public const uint SizeInBytes = 20;
}