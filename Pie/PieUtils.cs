using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;
using Vortice.DXGI;

namespace Pie;

public static class PieUtils
{
    #region Public API
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 Normalize(this Color color) =>
        new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new PieException(message);
    }
    
    public static unsafe T[] Combine<T>(params T[][] data) where T : unmanaged
    {
        int totalSize = 0;
        for (int i = 0; i < data.Length; i++)
            totalSize += data[i].Length;
        T[] result = new T[totalSize];

        totalSize = 0;
        fixed (void* ptr = result)
        {
            for (int i = 0; i < data.Length; i++)
            {
                fixed (void* dataPtr = data[i])
                    Unsafe.CopyBlock((byte*) ptr + totalSize, dataPtr, (uint) (data[i].Length * sizeof(T)));

                totalSize += data[i].Length;
            }
        }

        return result;
    }

    /// <summary>
    /// Copy the given data to a section in unmanaged memory (useful for copying data to a mapped buffer in a safe
    /// context.)
    /// </summary>
    /// <param name="unmanagedPtr">The pointer to unmanaged memory.</param>
    /// <param name="offsetInBytes">The offset in bytes.</param>
    /// <param name="dataLengthInBytes">The data length in bytes.</param>
    /// <param name="data">The data itself.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CopyToUnmanaged<T>(IntPtr unmanagedPtr, int offsetInBytes, uint dataLengthInBytes, T[] data) where T : unmanaged
    {
        fixed (void* dat = data)
            Unsafe.CopyBlock((byte*) unmanagedPtr + offsetInBytes, dat, dataLengthInBytes);
    }

    
    /// <summary>
    /// Copy the given data to a section in unmanaged memory (useful for copying data to a mapped buffer in a safe
    /// context.)
    /// </summary>
    /// <param name="unmanagedPtr">The pointer to unmanaged memory.</param>
    /// <param name="offsetInBytes">The offset in bytes.</param>
    /// <param name="data">The data itself.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyToUnmanaged<T>(IntPtr unmanagedPtr, int offsetInBytes, T[] data) where T : unmanaged
    {
        CopyToUnmanaged(unmanagedPtr, offsetInBytes, (uint) (Unsafe.SizeOf<T>() * data.Length), data);
    }
    
    #endregion
    
    #region Internal API
    
    internal static Vortice.DXGI.Format ToDxgiFormat(Format format, bool shaderResource)
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

    internal static void CheckIfValid(TextureDescription description)
    {
        if (description.ArraySize < 1)
            throw new PieException("Array size must be at least 1.");
    }

    internal static int GetSizeMultiplier(Format format)
    {
        return format switch
        {
            Format.R8_UNorm => 1,
            Format.R8G8_UNorm => 2,
            Format.R8G8B8A8_UNorm => 4,
            Format.B8G8R8A8_UNorm => 4,
            Format.R16G16B16A16_Float => 8,
            Format.R32G32B32A32_Float => 16,
            Format.D24_UNorm_S8_UInt => 16,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal static void CheckIfValid(int expected, int received)
    {
        if (received != expected)
            throw new PieException($"{expected} bytes expected, {received} bytes received.");
    }
    
    #endregion
}