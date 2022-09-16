using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Outline
{
    public short NumContours;
    public short NumPoints;

    public FT_Vector* Points;
    public sbyte* Tags;
    public short* Contours;

    public int Flags;
}