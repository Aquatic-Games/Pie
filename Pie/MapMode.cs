namespace Pie;

/// <summary>
/// The mode for which a buffer will be mapped.
/// </summary>
public enum MapMode
{
    /// <summary>
    /// The buffer will be mapped for reading only.
    /// </summary>
    Read,
    
    /// <summary>
    /// The buffer will be mapped for writing only.
    /// </summary>
    Write,
    
    /// <summary>
    /// The buffer will be mapped for both reading and writing.
    /// </summary>
    ReadWrite
}