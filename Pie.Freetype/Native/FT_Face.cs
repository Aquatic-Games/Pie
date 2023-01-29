using System;
using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Face
{
    public FT_Long NumFaces;
    public FT_Long FaceIndex;

    public FT_Long FaceFlags;
    public FT_Long StyleFlags;

    public FT_Long NumGlyphs;

    public FT_String* FamilyName;
    public FT_String* StyleName;

    public FT_Int NumFixedSizes;
    public FT_Bitmap_Size* AvailableSizes;

    public FT_Int NumCharmaps;
    public FT_CharMap** Charmaps;

    public FT_Generic Generic;

    public FT_BBox BBox;

    public FT_UShort UnitsPerEM;
    public FT_Short Ascender;
    public FT_Short Descender;
    public FT_Short Height;

    public FT_Short MaxAdvanceWidth;
    public FT_Short MaxAdvanceHeight;

    public FT_Short UnderlinePosition;
    public FT_Short UnderlineThickness;

    public FT_GlyphSlot* Glyph;
    public FT_Size* Size;
    public FT_CharMap* Charmap;

    private IntPtr Driver;
    private FT_Memory Memory;
    private FT_Stream Stream;

    private FT_List SizesList;

    private FT_Generic AutoHint;
    private void* Extensions;
    
    private IntPtr Internal;
}