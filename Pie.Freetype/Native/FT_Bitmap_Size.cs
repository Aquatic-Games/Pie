using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Bitmap_Size
{
    public FT_Short Height;
    public FT_Short Width;

    public FT_Pos Size;

    public FT_Pos Xppem;
    public FT_Pos Yppem;
}