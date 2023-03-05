using System.Runtime.InteropServices;

namespace Pie.Audio;

/// <summary>
/// Describes the format of PCM audio, such as its data type, and its sample rate.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct AudioFormat
{
    /// <summary>
    /// The number of channels the audio has.
    /// </summary>
    public byte Channels;
    
    /// <summary>
    /// The sampling rate of the audio.
    /// </summary>
    public int SampleRate;
    
    /// <summary>
    /// The <see cref="Pie.Audio.FormatType"/> of the audio.
    /// </summary>
    public FormatType FormatType;

    /// <summary>
    /// Create a new <see cref="AudioFormat"/>.
    /// </summary>
    /// <param name="channels">The number of channels the audio has.</param>
    /// <param name="sampleRate">The sampling rate of the audio.</param>
    /// <param name="formatType">The <see cref="Pie.Audio.FormatType"/> of the audio.</param>
    public AudioFormat(byte channels, int sampleRate, FormatType formatType)
    {
        Channels = channels;
        SampleRate = sampleRate;
        FormatType = formatType;
    }
}