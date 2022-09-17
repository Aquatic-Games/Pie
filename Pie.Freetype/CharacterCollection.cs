using System.Collections.Generic;
using Pie.Freetype.Native;
using static Pie.Freetype.Native.FreetypeNative;

namespace Pie.Freetype;

public unsafe class CharacterCollection
{
    private FT_Face* _face;
    //private Dictionary<char, Character> _characters;
    
    internal int Size;

    internal CharacterCollection(FT_Face* face)
    {
        //_characters = new Dictionary<char, Character>();
        _face = face;
    }

    public Character this[char c]
    {
        get
        {
            //if (!_characters.TryGetValue(c, out Character chr))
            //{
                FT_Set_Pixel_Sizes(_face, 0, (ushort) Size);
                FT_Load_Char(_face, new FT_ULong(c), LoadRender);
                FT_GlyphSlot* glyph = _face->Glyph;
                FT_Bitmap bitmap = glyph->Bitmap;
                byte[] data = new byte[bitmap.Width * bitmap.Rows * 4];
                // Convert to RGBA.
                // TODO: Add setting to enable/disable RGBA conversion.
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Rows; y++)
                    {
                        int pos = (int) (y * bitmap.Width + x);
                        data[pos * 4 + 0] = 255;
                        data[pos * 4 + 1] = 255;
                        data[pos * 4 + 2] = 255;
                        data[pos * 4 + 3] = bitmap.Buffer[pos];
                    }
                }
                
                Character chr = new Character()
                {
                    Width = (int) bitmap.Width,
                    Height = (int) bitmap.Rows,
                    Bitmap = data,
                    Advance = (int) glyph->Advance.X.Value >> 6,
                    BitmapLeft = glyph->BitmapLeft,
                    BitmapTop = glyph->BitmapTop
                };
                
                //_characters.Add(c, chr);
            //}

            return chr;
        }
    }
}