using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Size
{
    public FT_Face* Face;
    public FT_Generic Generic;
    public FT_Size_Metrics Metrics;
    public FT_Library Internal;
}