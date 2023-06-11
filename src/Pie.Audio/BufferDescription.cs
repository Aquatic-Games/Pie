namespace Pie.Audio;

public struct BufferDescription
{
    public AudioFormat Format;

    public BufferDescription(AudioFormat format)
    {
        Format = format;
    }
}