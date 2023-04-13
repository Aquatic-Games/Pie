using System.Numerics;
using System.Runtime.InteropServices;

namespace Pie.Utils;

[StructLayout(LayoutKind.Sequential)]
public struct VertexPositionTextureNormal
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public Vector3 Normal;

    public VertexPositionTextureNormal(Vector3 position, Vector2 texCoord, Vector3 normal)
    {
        Position = position;
        TexCoord = texCoord;
        Normal = normal;
    }

    public const uint SizeInBytes = 32;
}