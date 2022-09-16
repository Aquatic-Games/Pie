global using FT_Long = System.Runtime.InteropServices.CLong;
global using FT_ULong = System.Runtime.InteropServices.CULong;
global using FT_Pos = System.Runtime.InteropServices.CLong;
global using FT_Fixed = System.Runtime.InteropServices.CLong;
global using FT_Library = System.IntPtr;
global using FT_Int = System.Int32;
global using FT_UInt = System.UInt32;
global using FT_Short = System.Int16;
global using FT_UShort = System.UInt16;
global using FT_String = System.SByte;
global using FT_Byte = System.Byte;

using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

public static unsafe class FreetypeNative
{
    public const string LibraryName = "freetype";

    public const int LoadRender = 1 << 2;

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Init_FreeType(out FT_Library library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Done_FreeType(FT_Library library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern FT_Error FT_New_Face(FT_Library library, string path, FT_Long index, out FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_New_Memory_Face(FT_Library library, FT_Byte* @base, FT_Long fileSize,
        FT_Long faceIndex, out FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Done_Face(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Load_Char(FT_Face* face, FT_ULong charCode, FT_Int loadFlags);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern FT_Error FT_Set_Pixel_Sizes(FT_Face* face, FT_UInt pixelWidth, FT_UShort pixelHeight);
}