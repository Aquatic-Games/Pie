using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Stream
{
    public byte* Base;
    public CULong Size;
    public CULong Pos;

    public FT_StreamDesc Descriptor;
    public FT_StreamDesc PathName;
    public delegate*<FT_Stream*, CULong, byte*, CULong, CULong> Read;
    public delegate*<FT_Stream*, void> Close;

    public FT_Memory* Memory;
    public byte* Cursor;
    public byte* Limit;
}