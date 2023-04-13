using System.Runtime.InteropServices;

namespace Pie.Audio;

/// <summary>
/// <see cref="BufferDescription"/>s describe how an <see cref="AudioBuffer"/> should be processed.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct BufferDescription
{
    /// <summary>
    /// The buffer's <see cref="Pie.Audio.DataType"/>.
    /// </summary>
    public DataType DataType;
    
    /// <summary>
    /// The buffer's <see cref="Pie.Audio.AudioFormat"/>.
    /// </summary>
    public AudioFormat Format;

    /// <summary>
    /// Create a new <see cref="BufferDescription"/>.
    /// </summary>
    /// <param name="dataType">The buffer's <see cref="Pie.Audio.DataType"/>.</param>
    /// <param name="format">The buffer's <see cref="Pie.Audio.AudioFormat"/>.</param>
    public BufferDescription(DataType dataType, AudioFormat format)
    {
        DataType = dataType;
        Format = format;
    }
}