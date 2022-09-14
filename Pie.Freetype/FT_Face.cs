using System.Runtime.InteropServices;
using FT_Long = System.Runtime.InteropServices.CLong;
using FT_String = System.Byte;
using FT_Int = System.Int32;
using FT_UShort = System.UInt16;
using FT_Short = System.Int16;

namespace Pie.Freetype;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Face
{
    public FT_Long NumFaces;
    public FT_Long FaceIndex;

    public FT_Long FaceFlags;
    public FT_Long StyleFlags;

    public FT_Long NumGlyphs;

    public FT_String* FamilyName;
    public FT_String StyleName;

    public FT_Int NumFixedSizes;
    public FT_Bitmap_Size* AvailableSizes;

    public FT_Int NumCharmaps;
    public IntPtr Charmaps; // TODO: FT_CharMap

    public IntPtr Generic; // TODO: FT_Generic

    public FT_BBox BBox;

    public FT_UShort UnitsPerEM;
    public FT_Short Ascender;
    public FT_Short Descender;
    public FT_Short Height;

    public FT_Short MaxAdvanceWidth;
    public FT_Short MaxAdvanceHeight;

    public FT_Short UnderlinePosition;
    public FT_Short UnderlineThickness;
    
    
}