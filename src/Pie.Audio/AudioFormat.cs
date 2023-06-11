namespace Pie.Audio;

public struct AudioFormat
{
    public DataType DataType;
    public uint SampleRate;
    public byte Channels;

    public AudioFormat()
    {
        DataType = DataType.F32;
        SampleRate = 48000;
        Channels = 2;
    }

    public AudioFormat(DataType dataType, uint sampleRate, byte channels)
    {
        DataType = dataType;
        SampleRate = sampleRate;
        Channels = channels;
    }
}