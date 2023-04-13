namespace Pie.Audio;

/// <summary>
/// Represents all the different supported PCM data formats.
/// </summary>
public enum FormatType
{
    /// <summary>
    /// Unsigned 8-bit.
    /// </summary>
    U8,
    
    /// <summary>
    /// Signed 8-bit.
    /// </summary>
    I8,
    
    /// <summary>
    /// Unsigned 16-bit.
    /// </summary>
    U16,
    
    /// <summary>
    /// Signed 16-bit.
    /// </summary>
    I16,
    
    /// <summary>
    /// Signed 32-bit.
    /// </summary>
    I32,
    
    /// <summary>
    /// Signed 32-bit float, between -1 and 1.
    /// </summary>
    F32,
    
    /// <summary>
    /// Signed 64-bit float, between -1 and 1.
    /// </summary>
    F64
}