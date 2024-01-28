using System;
using Silk.NET.OpenGL;

namespace Pie.OpenGL;

internal static class GlUtils
{
    internal static MapBufferAccessMask ToGlMapMode(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Read => MapBufferAccessMask.ReadBit,
            MapMode.Write => MapBufferAccessMask.WriteBit,
            MapMode.ReadWrite => MapBufferAccessMask.ReadBit | MapBufferAccessMask.WriteBit,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}