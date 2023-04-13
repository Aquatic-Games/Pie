namespace Pie.Audio;

/// <summary>
/// A data store used when playing audio.
/// </summary>
public struct AudioBuffer
{
    /// <summary>
    /// A handle to the underlying mixr buffer.
    /// </summary>
    public readonly int Handle;

    /// <summary>
    /// Create a new <see cref="AudioBuffer"/> from the given handle.
    /// </summary>
    /// <param name="handle">A handle to the underlying mixr buffer.</param>
    public AudioBuffer(int handle)
    {
        Handle = handle;
    }
}