using System.Runtime.InteropServices;

namespace Pie.Text.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Vector
{
    public FT_Pos X;
    public FT_Pos Y;
}