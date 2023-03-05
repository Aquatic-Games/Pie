using System.Runtime.InteropServices;

namespace Pie.Audio;

[StructLayout(LayoutKind.Sequential)]
public struct AudioFormat
{
    public byte Channels;
    public int SampleRate;
    public FormatType FormatType;

    public AudioFormat(byte channels, int sampleRate, FormatType formatType)
    {
        Channels = channels;
        SampleRate = sampleRate;
        FormatType = formatType;
    }
}