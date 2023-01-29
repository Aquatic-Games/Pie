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
    public const string LibraryName = "libfreetype";
    private const CallingConvention Convention = System.Runtime.InteropServices.CallingConvention.Cdecl;

    public const int FaceFlagScalable = 1 << 0;
    public const int FaceFlagFixedSizes = 1 << 1;
    public const int FaceFlagFixedWidth = 1 << 2;
    public const int FaceFlagSFnt = 1 << 3;
    public const int FaceFlagHorizontal = 1 << 4;
    public const int FaceFlagVertical = 1 << 5;
    public const int FaceFlagKerning = 1 << 6;
    public const int FaceFlagFastGlyphs = 1 << 7;
    public const int FaceFlagMultipleMasters = 1 << 8;
    public const int FaceFlagGlyphNames = 1 << 9;
    public const int FaceFlagExternalStream = 1 << 10;
    public const int FaceFlagHinter = 1 << 11;
    public const int FaceFlagCidKeyed = 1 << 12;
    public const int FaceFlagTricky = 1 << 13;
    public const int FaceFlagColor = 1 << 14;
    public const int FaceFlagVariation = 1 << 15;
    public const int FaceFlagSvg = 1 << 16;
    public const int FaceFlagSBix = 1 << 17;
    public const int FaceFlagSBixOverlay = 1 << 18;

    public const int StyleFlagItalic = 1 << 0;
    public const int StyleFlagBold = 1 << 1;

    public const int OpenMemory = 1;
    public const int OpenStream = 2;
    public const int OpenPathname = 4;
    public const int OpenDriver = 8;
    public const int OpenParams = 16;

    public const int LoadDefault = 0;
    public const int LoadNoScale = 1 << 0;
    public const int LoadNoHinting = 1 << 1;
    public const int LoadRender = 1 << 2;
    public const int LoadNoBitmap = 1 << 3;
    public const int LoadVerticalLayout = 1 << 4;
    public const int LoadForceAutohint = 1 << 5;
    public const int LoadCropBitmap = 1 << 6;
    public const int LoadPedantic = 1 << 7;
    public const int LoadIgnoreGlobalAdvanceWidth = 1 << 9;
    public const int LoadNoRecurse = 1 << 10;
    public const int LoadIgnoreTransform = 1 << 11;
    public const int LoadMonochrome = 1 << 12;
    public const int LoadLinearDesign = 1 << 13;
    public const int LoadSBitsOnly = 1 << 14;
    public const int LoadNoAutohint = 1 << 15;
    public const int LoadColor = 1 << 20;
    public const int LoadComputeMetrics = 1 << 21;
    public const int LoadBitmapMetricsOnly = 1 << 22;

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Init_FreeType(out FT_Library library);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Done_FreeType(FT_Library library);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_New_Face(FT_Library library, sbyte* path, FT_Long index, out FT_Face* face);
    
    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Done_Face(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Reference_Face(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_New_Memory_Face(FT_Library library, FT_Byte* file, FT_Long fileSize,
        FT_Long faceIndex, out FT_Face* face);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Face_Properties(FT_Face* face, FT_UInt numProperties, FT_Parameter* properties);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error
        FT_Open_Face(FT_Library library, FT_Open_Args* args, FT_Long faceIndex, FT_Face* face);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Attach_File(FT_Face* face, sbyte* filePathName);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Attach_Stream(FT_Face* face, FT_Open_Args* parameters);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Set_Char_Size(FT_Face* face, CLong charWidth, CLong charHeight,
        FT_UInt horzResolution, FT_UInt vertResolution);
    
    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Set_Pixel_Sizes(FT_Face* face, FT_UInt pixelWidth, FT_UInt pixelHeight);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Request_Size(FT_Face* face, FT_Size_Request* request);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Select_Size(FT_Face* face, FT_Int strikeIndex);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Set_Transform(FT_Face* face, FT_Matrix* matrix, FT_Vector* delta);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern void FT_Get_Transform(FT_Face* face, FT_Matrix* matrix, FT_Vector* delta);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Load_Glyph(FT_Face* face, FT_UInt glyphIndex, int loadFlags);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_UInt FT_Get_Char_Index(FT_Face* face, FT_ULong charcode);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_ULong FT_Get_First_Char(FT_Face* face, FT_UInt* index);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_ULong FT_Get_Next_Char(FT_Face* face, FT_ULong charCode, FT_UInt* index);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_UInt FT_Get_Name_Index(FT_Face* face, FT_String* glyphName);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Load_Char(FT_Face* face, FT_ULong charCode, int loadFlags);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Render_Glyph(FT_GlyphSlot* slot, FT_Render_Mode renderMode);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Get_Kerning(FT_Face* face, FT_UInt leftGlyph, FT_UInt rightGlyph, FT_UInt kernMode,
        FT_Vector* kerning);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Get_Track_Kerning(FT_Face* face, CLong pointSize, FT_Int degree, CLong* kerning);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Get_Glyph_Name(FT_Face* face, FT_UInt glyphIndex, void* buffer, FT_UInt bufferMax);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern sbyte* FT_Get_Postscript_Name(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Select_Charmap(FT_Face* face, FT_Encoding encoding);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Set_Charmap(FT_Face* face, FT_CharMap* charmap);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Int FT_Get_Charmap_Index(FT_CharMap* charmap);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_UShort FT_Get_FSType_Flags(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = Convention)]
    public static extern FT_Error FT_Get_SubGlyph_Info(FT_GlyphSlot* glyph, FT_UInt subIndex, FT_Int* index,
        FT_UInt* flags, FT_Int* arg1, FT_Int* arg2, FT_Matrix* transform);

    public static FT_Render_Mode LoadTargetMode(int x)
    {
        return (FT_Render_Mode) ((x >> 16) & 15);
    }

    public static int LoadTarget(FT_Render_Mode x)
    {
        return ((int) x & 15) << 16;
    }
}