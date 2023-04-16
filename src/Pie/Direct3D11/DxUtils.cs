using System;

namespace Pie.Direct3D11;

internal static class DxUtils
{
    internal static Vortice.DXGI.Format ToDxgiFormat(this Format format, bool shaderResource)
    {
        return format switch
        {
            Format.R8G8B8A8_UNorm => Vortice.DXGI.Format.R8G8B8A8_UNorm,
            Format.B8G8R8A8_UNorm => Vortice.DXGI.Format.B8G8R8A8_UNorm,
            Format.D24_UNorm_S8_UInt => shaderResource ? Vortice.DXGI.Format.R24G8_Typeless : Vortice.DXGI.Format.D24_UNorm_S8_UInt,
            Format.R8_UNorm => Vortice.DXGI.Format.R8_UNorm,
            Format.R8G8_UNorm => Vortice.DXGI.Format.R8G8_UNorm,
            Format.R16G16B16A16_Float => Vortice.DXGI.Format.R16G16B16A16_Float,
            Format.R32G32B32A32_Float => Vortice.DXGI.Format.R32G32B32A32_Float,
            Format.R16G16B16A16_UNorm => Vortice.DXGI.Format.R16G16B16A16_UNorm,
            Format.R16G16B16A16_SNorm => Vortice.DXGI.Format.R16G16B16A16_SNorm,
            Format.R16G16B16A16_SInt => Vortice.DXGI.Format.R16G16B16A16_SInt,
            Format.R16G16B16A16_UInt => Vortice.DXGI.Format.R16G16B16A16_UInt,
            Format.R32G32_SInt => Vortice.DXGI.Format.R32G32_SInt,
            Format.R32G32_UInt => Vortice.DXGI.Format.R32G32_UInt,
            Format.R32G32_Float => Vortice.DXGI.Format.R32G32_Float,
            Format.R32G32B32_SInt => Vortice.DXGI.Format.R32G32B32_SInt,
            Format.R32G32B32_UInt => Vortice.DXGI.Format.R32G32B32_UInt,
            Format.R32G32B32_Float => Vortice.DXGI.Format.R32G32B32_Float,
            Format.R32G32B32A32_SInt => Vortice.DXGI.Format.R32G32B32A32_SInt,
            Format.R32G32B32A32_UInt => Vortice.DXGI.Format.R32G32B32A32_UInt,
            Format.R8_SNorm => Vortice.DXGI.Format.R8_SNorm,
            Format.R8_SInt => Vortice.DXGI.Format.R8_SInt,
            Format.R8_UInt => Vortice.DXGI.Format.R8_UInt,
            Format.R8G8_SNorm => Vortice.DXGI.Format.R8G8_SNorm,
            Format.R8G8_SInt => Vortice.DXGI.Format.R8G8_SInt,
            Format.R8G8_UInt => Vortice.DXGI.Format.R8G8_UInt,
            Format.R8G8B8A8_SNorm => Vortice.DXGI.Format.R8G8B8A8_SNorm,
            Format.R8G8B8A8_SInt => Vortice.DXGI.Format.R8G8B8A8_SInt,
            Format.R8G8B8A8_UInt => Vortice.DXGI.Format.R8G8B8A8_UInt,
            Format.R16_UNorm => Vortice.DXGI.Format.R16_UNorm,
            Format.R16_SNorm => Vortice.DXGI.Format.R16_SNorm,
            Format.R16_SInt => Vortice.DXGI.Format.R16_SInt,
            Format.R16_UInt => Vortice.DXGI.Format.R16_UInt,
            Format.R16_Float => Vortice.DXGI.Format.R16_Float,
            Format.R16G16_UNorm => Vortice.DXGI.Format.R16G16_UNorm,
            Format.R16G16_SNorm => Vortice.DXGI.Format.R16G16_SNorm,
            Format.R16G16_SInt => Vortice.DXGI.Format.R16G16_SInt,
            Format.R16G16_UInt => Vortice.DXGI.Format.R16G16_UInt,
            Format.R16G16_Float => Vortice.DXGI.Format.R16G16_Float,
            Format.R32_SInt => Vortice.DXGI.Format.R32_SInt,
            Format.R32_UInt => Vortice.DXGI.Format.R32_UInt,
            Format.R32_Float => Vortice.DXGI.Format.R32_Float,
            Format.R8G8B8A8_UNorm_SRgb => Vortice.DXGI.Format.R8G8B8A8_UNorm_SRgb,
            Format.B8G8R8A8_UNorm_SRgb => Vortice.DXGI.Format.B8G8R8A8_UNorm_SRgb,
            Format.D32_Float => shaderResource ? Vortice.DXGI.Format.R32_Typeless : Vortice.DXGI.Format.D32_Float,
            Format.D16_UNorm => shaderResource ? Vortice.DXGI.Format.R16_Typeless : Vortice.DXGI.Format.D16_UNorm,
            Format.BC1_UNorm => Vortice.DXGI.Format.BC1_UNorm,
            Format.BC1_UNorm_SRgb => Vortice.DXGI.Format.BC1_UNorm_SRgb,
            Format.BC2_UNorm => Vortice.DXGI.Format.BC2_UNorm,
            Format.BC2_UNorm_SRgb => Vortice.DXGI.Format.BC2_UNorm_SRgb,
            Format.BC3_UNorm => Vortice.DXGI.Format.BC3_UNorm,
            Format.BC3_UNorm_SRgb => Vortice.DXGI.Format.BC3_UNorm_SRgb,
            Format.BC4_UNorm => Vortice.DXGI.Format.BC4_UNorm,
            Format.BC4_SNorm => Vortice.DXGI.Format.BC4_SNorm,
            Format.BC5_UNorm => Vortice.DXGI.Format.BC5_UNorm,
            Format.BC5_SNorm => Vortice.DXGI.Format.BC5_SNorm,
            Format.BC6H_UF16 => Vortice.DXGI.Format.BC6H_Uf16,
            Format.BC6H_SF16 => Vortice.DXGI.Format.BC6H_Sf16,
            Format.BC7_UNorm => Vortice.DXGI.Format.BC7_UNorm,
            Format.BC7_UNorm_SRgb => Vortice.DXGI.Format.BC7_UNorm_SRgb,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    internal static Vortice.Direct3D11.MapMode ToDx11MapMode(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Read => Vortice.Direct3D11.MapMode.Read,
            MapMode.Write => Vortice.Direct3D11.MapMode.WriteDiscard,
            MapMode.ReadWrite => Vortice.Direct3D11.MapMode.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}