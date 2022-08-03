using System.Numerics;

namespace Pie.Utils;

public struct VertexPositionTextureNormal
{
    public Vector3 Position;
    public Vector2 TexCoords;
    public Vector3 Normal;

    public VertexPositionTextureNormal(Vector3 position, Vector2 texCoords, Vector3 normal)
    {
        Position = position;
        TexCoords = texCoords;
        Normal = normal;
    }

    public const uint SizeInBytes = 32;
}