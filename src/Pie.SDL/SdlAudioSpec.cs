namespace Pie.SDL;

public unsafe struct SdlAudioSpec
{
    public int Freq;
    public ushort Format;
    public byte Channels;
    public byte Silence;
    public ushort Samples;
    public ushort Padding;
    public uint Size;
    public delegate*<void*, byte*, int, void> Callback;
    public void* UserData;
}