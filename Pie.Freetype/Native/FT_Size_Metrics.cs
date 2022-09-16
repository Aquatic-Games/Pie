using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Size_Metrics
{
    public FT_UShort Xppem;
    public FT_UShort Yppem;

    public FT_Fixed XScale;
    public FT_Fixed YScale;

    public FT_Pos Ascender;
    public FT_Pos Descender;
    public FT_Pos Height;
    public FT_Pos MaxAdvance;
}