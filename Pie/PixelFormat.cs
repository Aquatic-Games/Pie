namespace Pie;

// TODO implement as many pixel formats as possible
public enum PixelFormat
{
    R8_UNorm,
    
    R8G8_UNorm,

    /// <summary>
    /// Unsigned byte RGBA format, with 8 bits for red, green, and blue, as well as 8 bits for alpha.
    /// </summary>
    R8G8B8A8_UNorm,

    /// <summary>
    /// Unsigned byte BGRA format, with 8 bits for blue, green, and red, as well as 8 bits for alpha.
    /// </summary>
    B8G8R8A8_UNorm,
    
    R16G16B16A16_Float,
    
    R32G32B32A32_Float,
    
    D24_UNorm_S8_UInt
}