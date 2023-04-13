using System.Numerics;
using System.Runtime.InteropServices;

namespace Pie.Utils;

[StructLayout(LayoutKind.Sequential)]
public struct VertexPosition
{
    public Vector3 Position;

    public VertexPosition(Vector3 position)
    {
        Position = position;
    }

    public const uint SizeInBytes = 12;
}