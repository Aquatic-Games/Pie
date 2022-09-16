using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Vector
{
    public FT_Pos X;
    public FT_Pos Y;
}