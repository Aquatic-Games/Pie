using System.Runtime.InteropServices;
using FT_Short = System.Int16;
using FT_Pos = System.Runtime.InteropServices.CLong;

namespace Pie.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Bitmap_Size
{
    public FT_Short Height;
    public FT_Short Width;

    public FT_Pos Size;

    public FT_Pos Xppem;
    public FT_Pos Yppem;
}