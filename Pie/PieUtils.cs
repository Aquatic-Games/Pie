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
    
    internal static Format ToDxgiFormat(PixelFormat format, bool shaderResource)
    {
        return format switch
        {
            PixelFormat.R8G8B8A8_UNorm => Format.R8G8B8A8_UNorm,
            PixelFormat.B8G8R8A8_UNorm => Format.B8G8R8A8_UNorm,
            PixelFormat.D24_UNorm_S8_UInt => shaderResource ? Format.R24G8_Typeless : Format.D24_UNorm_S8_UInt,
            PixelFormat.R8_UNorm => Format.R8_UNorm,
            PixelFormat.R8G8_UNorm => Format.R8G8_UNorm,
            PixelFormat.R16G16B16A16_Float => Format.R16G16B16A16_Float,
            PixelFormat.R32G32B32A32_Float => Format.R32G32B32A32_Float,
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

    internal static void CheckIfValid(int expected, int received)
    {
        if (received != expected)
            throw new PieException($"{expected} bytes expected, {received} bytes received.");
    }
    
    #endregion
}