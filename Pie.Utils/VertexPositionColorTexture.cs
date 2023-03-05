using System.Numerics;
using System.Runtime.InteropServices;

namespace Pie.Utils;

[StructLayout(LayoutKind.Sequential)]
public struct VertexPositionColorTexture
{
    public Vector3 Position;
    public Vector4 Color;
    public Vector2 TexCoord;

    public VertexPositionColorTexture(Vector3 position, Vector4 color, Vector2 texCoord)
    {
        Position = position;
        Color = color;
        TexCoord = texCoord;
    }

    public const uint SizeInBytes = 36;
}