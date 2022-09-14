using System.Runtime.InteropServices;
using FT_Long = System.Runtime.InteropServices.CLong;

namespace Pie.Freetype;

public static class FreetypeNative
{
    public const string LibraryName = "freetype2";

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Init_FreeType(out IntPtr library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Done_FreeType(IntPtr library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern FT_Error FT_New_Face(IntPtr library, string path, FT_Long index, out FT_Face face);
}