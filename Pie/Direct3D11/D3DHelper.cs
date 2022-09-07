using System;
using Vortice.DXGI;

namespace Pie.Direct3D11;

internal static class D3DHelper
{
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
}