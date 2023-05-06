using System;
using System.Runtime.CompilerServices;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;

namespace Pie.Direct3D11;

internal static class DxUtils
{
    internal static Silk.NET.DXGI.Format ToDxgiFormat(this Format format, bool shaderResource)
    {
        return format switch
        {
            Format.R8G8B8A8_UNorm => Silk.NET.DXGI.Format.FormatR8G8B8A8Unorm,
            Format.B8G8R8A8_UNorm => Silk.NET.DXGI.Format.FormatB8G8R8A8Unorm,
            Format.D24_UNorm_S8_UInt => shaderResource ? Silk.NET.DXGI.Format.FormatR24G8Typeless : Silk.NET.DXGI.Format.FormatD24UnormS8Uint,
            Format.R8_UNorm => Silk.NET.DXGI.Format.FormatR8Unorm,
            Format.R8G8_UNorm => Silk.NET.DXGI.Format.FormatR8G8Unorm,
            Format.R16G16B16A16_Float => Silk.NET.DXGI.Format.FormatR16G16B16A16Float,
            Format.R32G32B32A32_Float => Silk.NET.DXGI.Format.FormatR32G32B32A32Float,
            Format.R16G16B16A16_UNorm => Silk.NET.DXGI.Format.FormatR16G16B16A16Unorm,
            Format.R16G16B16A16_SNorm => Silk.NET.DXGI.Format.FormatR16G16B16A16SNorm,
            Format.R16G16B16A16_SInt => Silk.NET.DXGI.Format.FormatR16G16B16A16Sint,
            Format.R16G16B16A16_UInt => Silk.NET.DXGI.Format.FormatR16G16B16A16Uint,
            Format.R32G32_SInt => Silk.NET.DXGI.Format.FormatR32G32Sint,
            Format.R32G32_UInt => Silk.NET.DXGI.Format.FormatR32G32Uint,
            Format.R32G32_Float => Silk.NET.DXGI.Format.FormatR32G32Float,
            Format.R32G32B32_SInt => Silk.NET.DXGI.Format.FormatR32G32B32Sint,
            Format.R32G32B32_UInt => Silk.NET.DXGI.Format.FormatR32G32B32Uint,
            Format.R32G32B32_Float => Silk.NET.DXGI.Format.FormatR32G32B32Float,
            Format.R32G32B32A32_SInt => Silk.NET.DXGI.Format.FormatR32G32B32A32Sint,
            Format.R32G32B32A32_UInt => Silk.NET.DXGI.Format.FormatR32G32B32A32Uint,
            Format.R8_SNorm => Silk.NET.DXGI.Format.FormatR8SNorm,
            Format.R8_SInt => Silk.NET.DXGI.Format.FormatR8Sint,
            Format.R8_UInt => Silk.NET.DXGI.Format.FormatR8Uint,
            Format.R8G8_SNorm => Silk.NET.DXGI.Format.FormatR8G8SNorm,
            Format.R8G8_SInt => Silk.NET.DXGI.Format.FormatR8G8Sint,
            Format.R8G8_UInt => Silk.NET.DXGI.Format.FormatR8G8Uint,
            Format.R8G8B8A8_SNorm => Silk.NET.DXGI.Format.FormatR8G8B8A8SNorm,
            Format.R8G8B8A8_SInt => Silk.NET.DXGI.Format.FormatR8G8B8A8Sint,
            Format.R8G8B8A8_UInt => Silk.NET.DXGI.Format.FormatR8G8B8A8Uint,
            Format.R16_UNorm => Silk.NET.DXGI.Format.FormatR16Unorm,
            Format.R16_SNorm => Silk.NET.DXGI.Format.FormatR16SNorm,
            Format.R16_SInt => Silk.NET.DXGI.Format.FormatR16Sint,
            Format.R16_UInt => Silk.NET.DXGI.Format.FormatR16Uint,
            Format.R16_Float => Silk.NET.DXGI.Format.FormatR16Float,
            Format.R16G16_UNorm => Silk.NET.DXGI.Format.FormatR16G16Unorm,
            Format.R16G16_SNorm => Silk.NET.DXGI.Format.FormatR16G16SNorm,
            Format.R16G16_SInt => Silk.NET.DXGI.Format.FormatR16G16Sint,
            Format.R16G16_UInt => Silk.NET.DXGI.Format.FormatR16G16Uint,
            Format.R16G16_Float => Silk.NET.DXGI.Format.FormatR16G16Float,
            Format.R32_SInt => Silk.NET.DXGI.Format.FormatR32Sint,
            Format.R32_UInt => Silk.NET.DXGI.Format.FormatR32Uint,
            Format.R32_Float => Silk.NET.DXGI.Format.FormatR32Float,
            Format.R8G8B8A8_UNorm_SRgb => Silk.NET.DXGI.Format.FormatR8G8B8A8UnormSrgb,
            Format.B8G8R8A8_UNorm_SRgb => Silk.NET.DXGI.Format.FormatB8G8R8A8UnormSrgb,
            Format.D32_Float => shaderResource ? Silk.NET.DXGI.Format.FormatR32Typeless : Silk.NET.DXGI.Format.FormatD32Float,
            Format.D16_UNorm => shaderResource ? Silk.NET.DXGI.Format.FormatR16Typeless : Silk.NET.DXGI.Format.FormatD16Unorm,
            Format.BC1_UNorm => Silk.NET.DXGI.Format.FormatBC1Unorm,
            Format.BC1_UNorm_SRgb => Silk.NET.DXGI.Format.FormatBC1UnormSrgb,
            Format.BC2_UNorm => Silk.NET.DXGI.Format.FormatBC2Unorm,
            Format.BC2_UNorm_SRgb => Silk.NET.DXGI.Format.FormatBC2UnormSrgb,
            Format.BC3_UNorm => Silk.NET.DXGI.Format.FormatBC3Unorm,
            Format.BC3_UNorm_SRgb => Silk.NET.DXGI.Format.FormatBC3UnormSrgb,
            Format.BC4_UNorm => Silk.NET.DXGI.Format.FormatBC4Unorm,
            Format.BC4_SNorm => Silk.NET.DXGI.Format.FormatBC4SNorm,
            Format.BC5_UNorm => Silk.NET.DXGI.Format.FormatBC5Unorm,
            Format.BC5_SNorm => Silk.NET.DXGI.Format.FormatBC5SNorm,
            Format.BC6H_UF16 => Silk.NET.DXGI.Format.FormatBC6HUF16,
            Format.BC6H_SF16 => Silk.NET.DXGI.Format.FormatBC6HSF16,
            Format.BC7_UNorm => Silk.NET.DXGI.Format.FormatBC7Unorm,
            Format.BC7_UNorm_SRgb => Silk.NET.DXGI.Format.FormatBC7UnormSrgb,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    internal static Map ToDx11MapMode(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Read => Map.Read,
            MapMode.Write => Map.WriteDiscard,
            MapMode.ReadWrite => Map.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint CalcSubresource(uint mipSlice, uint arraySlice, uint mipLevels) =>
        mipSlice + (arraySlice * mipLevels);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Succeeded(int result, out HResult hresult)
    {
        hresult = (HResult) result;
        if (!hresult.IsSuccess)
        {
            Console.WriteLine("HRESULT code: " + hresult.Code);
            return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Succeeded(int result) => Succeeded(result, out _);
}