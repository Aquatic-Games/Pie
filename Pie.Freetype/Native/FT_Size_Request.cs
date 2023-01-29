using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Size_Request
{
    public FT_Size_Request_Type Type;
    public FT_Long Width;
    public FT_Long Height;
    public FT_UInt HoriResolution;
    public FT_UInt VertResolution;
}