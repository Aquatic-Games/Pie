using System.Runtime.InteropServices;
using FT_UInt = System.UInt32;
using FT_Fixed = System.Runtime.InteropServices.CLong;

namespace Pie.Freetype;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_GlyphSlot
{
    public IntPtr Library;
    public FT_Face* Face;
    public FT_GlyphSlot* Next;
    public FT_UInt GlyphIndex;
    public IntPtr Generic;

    public FT_Glyph_Metrics Metrics;
    public FT_Fixed LinearHoriAdvance;
    public FT_Fixed LinearVertAdvance;
    
}