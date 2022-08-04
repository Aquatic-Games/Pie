using System.Drawing;
using System.Numerics;

namespace Pie;

internal static class PieUtils
{
    public static Vector4 Normalize(this Color color) =>
        new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
}