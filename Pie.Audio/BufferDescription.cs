using System.Runtime.InteropServices;

namespace Pie.Audio;

[StructLayout(LayoutKind.Sequential)]
public struct BufferDescription
{
    public DataType DataType;
    public AudioFormat Format;

    public BufferDescription(DataType dataType, AudioFormat format)
    {
        DataType = dataType;
        Format = format;
    }
}