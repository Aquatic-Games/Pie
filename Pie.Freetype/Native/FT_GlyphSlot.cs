using System;
using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_GlyphSlot
{
    public FT_Library Library;
    public FT_Face* Face;
    public FT_GlyphSlot* Next;
    public FT_UInt GlyphIndex;
    public FT_Generic Generic;

    public FT_Glyph_Metrics Metrics;
    public FT_Fixed LinearHoriAdvance;
    public FT_Fixed LinearVertAdvance;
    public FT_Vector Advance;

    public FT_Glyph_Format Format;

    public FT_Bitmap Bitmap;
    public FT_Int BitmapLeft;
    public FT_Int BitmapTop;

    public FT_Outline Outline;

    public FT_UInt NumSubglyphs;
    public IntPtr Subglyphs; // NOTE: Subglyph info not yet implemented todo.

    public void* ControlData;
    public long ControlLen;

    public FT_Pos LsbDelta;
    public FT_Pos RsbDelta;

    public void* Other;

    private IntPtr Internal;
}