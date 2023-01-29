using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Parameter
{
    public FT_ULong Tag;
    public void* Data;
}