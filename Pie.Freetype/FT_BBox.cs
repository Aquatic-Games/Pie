using FT_Pos = System.Runtime.InteropServices.CLong;

namespace Pie.Freetype;

public struct FT_BBox
{
    public FT_Pos XMin, YMin;
    public FT_Pos XMax, YMax;
}