namespace Pie.Audio;

/// <summary>
/// Many functions return an <see cref="AudioResult"/>, and quietly fail if the result is not <see cref="AudioResult.Ok"/>.
/// </summary>
public enum AudioResult
{
    /// <summary>
    /// The action was a success.
    /// </summary>
    Ok,
        
    /// <summary>
    /// An invalid buffer was provided.
    /// </summary>
    InvalidBuffer,
    
    /// <summary>
    /// An invalid channel was provided.
    /// </summary>
    InvalidChannel,
    
    /// <summary>
    /// A value fell out of range of the supported values.
    /// </summary>
    OutOfRange
}