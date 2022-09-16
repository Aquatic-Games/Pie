using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_CharMap
{
    public FT_Face* Face;
    public FT_Encoding Encoding;
    public FT_UShort PlatformId;
    public FT_UShort EncodingId;
}