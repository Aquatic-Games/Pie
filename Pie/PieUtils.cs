using System;
using System.Drawing;
using System.Numerics;
using Vortice.DXGI;

namespace Pie;

internal static class PieUtils
{
    public static Vector4 Normalize(this Color color) =>
        new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    
    public static Format ToDxgiFormat(PixelFormat format, bool depthFlag)
    {
        return format switch
        {
            PixelFormat.R8G8B8A8_UNorm => Format.R8G8B8A8_UNorm,
            PixelFormat.B8G8R8A8_UNorm => Format.B8G8R8A8_UNorm,
            PixelFormat.D24_UNorm_S8_UInt => depthFlag ? Format.R24G8_Typeless : Format.D24_UNorm_S8_UInt,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static void CheckIfValid(TextureDescription description)
    {
        if (description.ArraySize < 1)
            throw new PieException("Array size must be at least 1.");
    }

    public static void CheckIfValid(int expected, int received)
    {
        if (received != expected)
            throw new PieException($"{expected} bytes expected, {received} bytes received.");
    }

    public static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new PieException(message);
    }
}