using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Pie.Text.Native;
using static Pie.Text.Native.FreetypeNative;

namespace Pie.Text;

public unsafe class Face : IDisposable
{
    private FT_Face* _face;
    private byte* _faceData;

    public readonly string Family;
    public readonly string Style;
    
    public readonly FaceFlags Flags;

    internal Face(FT_Face* face, byte* data, FaceFlags flags)
    {
        _face = face;
        _faceData = data;
        
        Family = Marshal.PtrToStringAnsi((IntPtr) face->FamilyName);
        Style = Marshal.PtrToStringAnsi((IntPtr) face->StyleName);
        
        Flags = flags;
    }

    public bool CharacterExists(char c)
    {
        return FT_Get_Char_Index(_face, new FT_ULong(c)) != 0;
    }

    public Character GetCharacter(char c, uint size)
    {
        FT_Error error;

        if ((error = FT_Set_Pixel_Sizes(_face, 0, (ushort) size)) != FT_Error.Ok)
            throw new Exception("Freetype failed: " + error);

        bool isMonochrome = (Flags & FaceFlags.Antialiased) != FaceFlags.Antialiased;
        
        int loadFlags = LoadRender;
        if (isMonochrome)
            loadFlags |= LoadMonochrome;

        if ((error = FT_Load_Char(_face, new FT_ULong(c), loadFlags)) != FT_Error.Ok)
            throw new Exception("Freetype failed: " + error);
        FT_GlyphSlot* glyph = _face->Glyph;
        FT_Bitmap bitmap = glyph->Bitmap;

        byte[] data;

        if (isMonochrome)
        {
            if ((Flags & FaceFlags.RgbaConvert) == FaceFlags.RgbaConvert)
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
            if ((Flags & FaceFlags.RgbaConvert) == FaceFlags.RgbaConvert)
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
    
    public void Dispose()
    {
        FT_Done_Face(_face);
        if (_faceData != null)
            NativeMemory.Free(_faceData);
    }
}