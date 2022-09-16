namespace Pie.Freetype.Native;

public enum FT_Glyph_Format : uint
{
    None = 0,
    Composite = ('c' << 24) | ('o' << 16) | ('m' << 8) | 'p',
    Bitmap = ('b' << 24) | ('i' << 16) | ('t' << 8) | 's',
    Outline = ('o' << 24) | ('u' << 16) | ('t' << 8) | 'l',
    Plotter = ('p' << 24) | ('l' << 16) | ('o' << 8) | 't',
    SVG = ('S' << 24) | ('V' << 16) | ('G' << 8) | ' '
}