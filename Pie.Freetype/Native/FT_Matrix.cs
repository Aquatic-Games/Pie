using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Matrix
{
    public FT_Long XX;
    public FT_Long XY;
    public FT_Long YX;
    public FT_Long YY;
}