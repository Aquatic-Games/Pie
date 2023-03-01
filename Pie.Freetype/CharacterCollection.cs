using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pie.Freetype.Native;
using static Pie.Freetype.Native.FreetypeNative;

namespace Pie.Freetype;

public unsafe class CharacterCollection
{
    private FT_Face* _face;
    //private Dictionary<char, Character> _characters;
    
    internal int Size;

    private FaceFlags _flags;

    internal CharacterCollection(FT_Face* face, FaceFlags flags)
    {
        //_characters = new Dictionary<char, Character>();
        _face = face;
        _flags = flags;
    }

    public Character this[char c]
    {
        get
        {
            //if (!_characters.TryGetValue(c, out Character chr))
            //{
                FT_Set_Pixel_Sizes(_face, 0, (ushort) Size);

                bool isMonochrome = (_flags & FaceFlags.Antialiased) != FaceFlags.Antialiased;
                
                int loadFlags = LoadRender;
                if (isMonochrome)
                    loadFlags |= LoadMonochrome;

                FT_Load_Char(_face, new FT_ULong(c), loadFlags);
                FT_GlyphSlot* glyph = _face->Glyph;
                FT_Bitmap bitmap = glyph->Bitmap;

                byte[] data;

                if (isMonochrome)
                {
                    if ((_flags & FaceFlags.RgbaConvert) == FaceFlags.RgbaConvert)
                    {
                        data = new byte[bitmap.Width * bitmap.Rows * 4];
                        // Convert to RGBA.
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            for (int y = 0; y < bitmap.Rows; y++)
                            {
                                byte* row = &bitmap.Buffer[bitmap.Pitch * y];
                                
                                int pos = (int) (y * bitmap.Width + x);
                                data[pos * 4 + 0] = 255;
                                data[pos * 4 + 1] = 255;
                                data[pos * 4 + 2] = 255;
                                data[pos * 4 + 3] = (byte) ((row[x >> 3] & (128 >> (x & 7))) != 0 ? 255 : 0); // WTF??
                            }
                        }
                    }
                    else
                    {
                        data = new byte[bitmap.Width * bitmap.Rows];
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            for (int y = 0; y < bitmap.Rows; y++)
                            {
                                byte* row = &bitmap.Buffer[bitmap.Pitch * y];
                                
                                int pos = (int) (y * bitmap.Width + x);
                                data[pos] = (byte) ((row[x >> 3] & (128 >> (x & 7))) != 0 ? 255 : 0);
                            }
                        }
                    }
                }
                else
                {
                    if ((_flags & FaceFlags.RgbaConvert) == FaceFlags.RgbaConvert)
                    {
                        data = new byte[bitmap.Width * bitmap.Rows * 4];
                        // Convert to RGBA.
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
                    }
                    else
                    {
                        // Just do a straight copy.
                        data = new byte[bitmap.Width * bitmap.Rows];
                        fixed (byte* dPtr = data)
                            Unsafe.CopyBlock(dPtr, bitmap.Buffer, (uint) data.Length);
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