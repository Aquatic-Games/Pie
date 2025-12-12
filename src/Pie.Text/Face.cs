using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreeTypeSharp;
using static FreeTypeSharp.FT_LOAD;
using static FreeTypeSharp.FT;

namespace Pie.Text;

public unsafe class Face : IDisposable
{
    private FT_FaceRec_* _face;
    private byte* _faceData;

    public readonly string Family;
    public readonly string Style;
    
    public readonly FaceFlags Flags;

    internal Face(FT_FaceRec_* face, byte* data, FaceFlags flags)
    {
        _face = face;
        _faceData = data;
        
        Family = Marshal.PtrToStringAnsi((IntPtr) face->family_name);
        Style = Marshal.PtrToStringAnsi((IntPtr) face->style_name);
        
        Flags = flags;
    }

    public bool CharacterExists(char c)
    {
        return FT_Get_Char_Index(_face, (nuint) c) != 0;
    }

    public Character GetCharacter(char c, uint size)
    {
        FT_Error error;

        if ((error = FT_Set_Pixel_Sizes(_face, 0, (ushort) size)) != FT_Error.FT_Err_Ok)
            throw new Exception("Freetype failed: " + error);

        bool isMonochrome = (Flags & FaceFlags.Antialiased) != FaceFlags.Antialiased;
        
        FT_LOAD loadFlags = FT_LOAD_RENDER;
        if (isMonochrome)
            loadFlags |= FT_LOAD_MONOCHROME;

        if ((error = FT_Load_Char(_face, (nuint) c, loadFlags)) != FT_Error.FT_Err_Ok)
            throw new Exception("Freetype failed: " + error);
        FT_GlyphSlotRec_* glyph = _face->glyph;
        FT_Bitmap_ bitmap = glyph->bitmap;

        byte[] data;

        if (isMonochrome)
        {
            if ((Flags & FaceFlags.RgbaConvert) == FaceFlags.RgbaConvert)
            {
                data = new byte[bitmap.width * bitmap.rows * 4];
                // Convert to RGBA.
                for (int x = 0; x < bitmap.width; x++)
                {
                    for (int y = 0; y < bitmap.rows; y++)
                    {
                        byte* row = &bitmap.buffer[bitmap.pitch * y];
                        
                        int pos = (int) (y * bitmap.width + x);
                        data[pos * 4 + 0] = 255;
                        data[pos * 4 + 1] = 255;
                        data[pos * 4 + 2] = 255;
                        data[pos * 4 + 3] = (byte) ((row[x >> 3] & (128 >> (x & 7))) != 0 ? 255 : 0); // WTF??
                    }
                }
            }
            else
            {
                data = new byte[bitmap.width * bitmap.rows];
                for (int x = 0; x < bitmap.width; x++)
                {
                    for (int y = 0; y < bitmap.rows; y++)
                    {
                        byte* row = &bitmap.buffer[bitmap.pitch * y];
                        
                        int pos = (int) (y * bitmap.width + x);
                        data[pos] = (byte) ((row[x >> 3] & (128 >> (x & 7))) != 0 ? 255 : 0);
                    }
                }
            }
        }
        else
        {
            if ((Flags & FaceFlags.RgbaConvert) == FaceFlags.RgbaConvert)
            {
                data = new byte[bitmap.width * bitmap.rows * 4];
                // Convert to RGBA.
                for (int x = 0; x < bitmap.width; x++)
                {
                    for (int y = 0; y < bitmap.rows; y++)
                    {
                        int pos = (int) (y * bitmap.width + x);
                        data[pos * 4 + 0] = 255;
                        data[pos * 4 + 1] = 255;
                        data[pos * 4 + 2] = 255;
                        data[pos * 4 + 3] = bitmap.buffer[pos];
                    }
                }
            }
            else
            {
                // Just do a straight copy.
                data = new byte[bitmap.width * bitmap.rows];
                fixed (byte* dPtr = data)
                    Unsafe.CopyBlock(dPtr, bitmap.buffer, (uint) data.Length);
            }
        }

        Character chr = new Character()
        {
            Width = (int) bitmap.width,
            Height = (int) bitmap.rows,
            Bitmap = data,
            Advance = (int) glyph->advance.x >> 6,
            BitmapLeft = glyph->bitmap_left,
            BitmapTop = glyph->bitmap_top
        };
            
            //_characters.Add(c, chr);
        //}

        return chr;
    }
    
    public void Dispose()
    {
        FT_Done_Face(_face);
        if (_faceData != null)
            NativeMemory.Free(_faceData);
    }
}