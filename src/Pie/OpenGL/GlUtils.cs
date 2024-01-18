using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal static class GlUtils
{
    internal static BufferAccessMask ToGlMapMode(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Read => BufferAccessMask.MapReadBit,
            MapMode.Write => BufferAccessMask.MapWriteBit,
            MapMode.ReadWrite => BufferAccessMask.MapReadBit | BufferAccessMask.MapWriteBit,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}