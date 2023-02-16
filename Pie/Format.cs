// ReSharper disable InconsistentNaming
namespace Pie;

/// <summary>
/// Used to describe the type of input data passed to an object. Used in <see cref="Texture"/>s and <see cref="InputLayout"/>s.
/// </summary>
public enum Format
{
    /// <summary>
    /// 1-component unsigned byte, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8_UNorm,
    
    /// <summary>
    /// 1-component signed byte, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8_SNorm,
    
    /// <summary>
    /// 1-component signed byte.
    /// </summary>
    R8_SInt,
    
    /// <summary>
    /// 1-component unsigned byte.
    /// </summary>
    R8_UInt,
    
    /// <summary>
    /// 2-component unsigned byte, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8G8_UNorm,
    
    /// <summary>
    /// 1-component signed byte, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8G8_SNorm,
    
    /// <summary>
    /// 2-component signed byte.
    /// </summary>
    R8G8_SInt,
    
    /// <summary>
    /// 1-component unsigned byte.
    /// </summary>
    R8G8_UInt,

    /// <summary>
    /// 4-component unsigned byte, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8G8B8A8_UNorm,
    
    /// <summary>
    /// 4-component unsigned byte, in sRGB color space, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8G8B8A8_UNorm_SRgb,
    
    /// <summary>
    /// 4-component signed byte, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R8G8B8A8_SNorm,
    
    /// <summary>
    /// 4-component signed byte.
    /// </summary>
    R8G8B8A8_SInt,
    
    /// <summary>
    /// 4-component unsigned byte.
    /// </summary>
    R8G8B8A8_UInt,
    
    /// <summary>
    /// 4-component unsigned byte, that is normalized to 0-1 when passed to the object. The data will be in BGRA format.
    /// </summary>
    B8G8R8A8_UNorm,
    
    /// <summary>
    /// 4-component unsigned byte, in sRGB color space, that is normalized to 0-1 when passed to the object. The data will be in BGRA format.
    /// </summary>
    B8G8R8A8_UNorm_SRgb,
    
    /// <summary>
    /// 1-component unsigned short, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R16_UNorm,
    
    /// <summary>
    /// 1-component signed short, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R16_SNorm,
    
    /// <summary>
    /// 1-component signed short.
    /// </summary>
    R16_SInt,
    
    /// <summary>
    /// 1-component unsigned short.
    /// </summary>
    R16_UInt,
    
    /// <summary>
    /// 1-component half float.
    /// </summary>
    R16_Float,
    
    /// <summary>
    /// 2-component unsigned short, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R16G16_UNorm,
    
    /// <summary>
    /// 2-component signed short, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R16G16_SNorm,
    
    /// <summary>
    /// 2-component signed short.
    /// </summary>
    R16G16_SInt,
    
    /// <summary>
    /// 2-component unsigned short.
    /// </summary>
    R16G16_UInt,
    
    /// <summary>
    /// 2-component half float.
    /// </summary>
    R16G16_Float,

    /// <summary>
    /// 4-component unsigned short, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R16G16B16A16_UNorm,
    
    /// <summary>
    /// 4-component signed short, that is normalized to 0-1 when passed to the object.
    /// </summary>
    R16G16B16A16_SNorm,
    
    /// <summary>
    /// 4-component signed short.
    /// </summary>
    R16G16B16A16_SInt,
    
    /// <summary>
    /// 4-component unsigned short.
    /// </summary>
    R16G16B16A16_UInt,
    
    /// <summary>
    /// 4-component half float.
    /// </summary>
    R16G16B16A16_Float,
    
    /// <summary>
    /// 1-component signed 32-bit integer.
    /// </summary>
    R32_SInt,
    
    /// <summary>
    /// 1-component unsigned 32-bit integer.
    /// </summary>
    R32_UInt,
    
    /// <summary>
    /// 1-component 32-bit float.
    /// </summary>
    R32_Float,
    
    /// <summary>
    /// 2-component signed 32-bit integer.
    /// </summary>
    R32G32_SInt,
    
    /// <summary>
    /// 2-component unsigned 32-bit integer.
    /// </summary>
    R32G32_UInt,
    
    /// <summary>
    /// 2-component 32-bit float.
    /// </summary>
    R32G32_Float,
    
    /// <summary>
    /// 3-component signed 32-bit integer.
    /// </summary>
    R32G32B32_SInt,
    
    /// <summary>
    /// 3-component unsigned 32-bit integer.
    /// </summary>
    R32G32B32_UInt,
    
    /// <summary>
    /// 3-component 32-bit float.
    /// </summary>
    R32G32B32_Float,
    
    /// <summary>
    /// 4-component signed 32-bit integer.
    /// </summary>
    R32G32B32A32_SInt,
    
    /// <summary>
    /// 4-component unsigned 32-bit integer.
    /// </summary>
    R32G32B32A32_UInt,
    
    /// <summary>
    /// 4-component 32-bit float.
    /// </summary>
    R32G32B32A32_Float,
    
    /// <summary>
    /// 24 bits for the depth, 8 bits for the stencil.
    /// </summary>
    D24_UNorm_S8_UInt,
    
    /// <summary>
    /// 32-bit depth.
    /// </summary>
    D32_Float,
    
    /// <summary>
    /// 16-bit depth, that is normalized to 0-1 when passed to the object.
    /// </summary>
    D16_UNorm,
    
    /// <summary>
    /// BC1 (DXT1) compression.
    /// </summary>
    BC1_UNorm,
    
    /// <summary>
    /// BC1 (DXT1) compression, with sRGB.
    /// </summary>
    BC1_UNorm_SRgb,
    
    /// <summary>
    /// BC2 (DXT3) compression.
    /// </summary>
    BC2_UNorm,
    
    /// <summary>
    /// BC2 (DXT3) compression, with sRGB.
    /// </summary>
    BC2_UNorm_SRgb,
    
    /// <summary>
    /// BC3 (DXT5) compression.
    /// </summary>
    BC3_UNorm,
    
    /// <summary>
    /// BC3 (DXT5) compression, with sRGB.
    /// </summary>
    BC3_UNorm_SRgb,
    
    /// <summary>
    /// BC4 compression.
    /// </summary>
    BC4_UNorm,
    
    /// <summary>
    /// BC4 compression.
    /// </summary>
    BC4_SNorm,
    
    /// <summary>
    /// BC5 compression.
    /// </summary>
    BC5_UNorm,
    
    /// <summary>
    /// BC5 compression.
    /// </summary>
    BC5_SNorm,
    
    /// <summary>
    /// BC6H compression.
    /// </summary>
    BC6H_UF16,
    
    /// <summary>
    /// BC6H compression.
    /// </summary>
    BC6H_SF16,
    
    /// <summary>
    /// BC7 compression.
    /// </summary>
    BC7_UNorm,
    
    /// <summary>
    /// BC7 compression, with sRGB.
    /// </summary>
    BC7_UNorm_SRgb
}