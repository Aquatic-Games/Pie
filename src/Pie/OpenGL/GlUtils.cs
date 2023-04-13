using System;
using Silk.NET.OpenGL;

namespace Pie.OpenGL;

internal static class GlUtils
{
    internal static BufferAccessARB ToGlMapMode(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Read => BufferAccessARB.ReadOnly,
            MapMode.Write => BufferAccessARB.WriteOnly,
            MapMode.ReadWrite => BufferAccessARB.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}