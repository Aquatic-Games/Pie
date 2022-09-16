using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Bitmap
{
    public uint Rows;
    public uint Width;
    public int Pitch;
    public byte* Buffer;
    public ushort NumGrays;
    public byte PixelMode;
    public byte PaletteMode;
    public void* Palette;
}