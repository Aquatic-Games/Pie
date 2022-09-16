using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_BBox
{
    public FT_Pos XMin, YMin;
    public FT_Pos XMax, YMax;
}