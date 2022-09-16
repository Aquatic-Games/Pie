using System;
using Pie.Freetype.Native;

namespace Pie.Freetype;

public unsafe class Face : IDisposable
{
    private FT_Face* _face;
    private int _size;

    public int Size
    {
        get => _size;
        set
        {
            _size = value;
            Characters.Size = value;
        }
    }

    public CharacterCollection Characters { get; }

    internal Face(FT_Face* face)
    {
        _face = face;
        Characters = new CharacterCollection(_face);
        Size = 0;
    }
    
    public void Dispose()
    {
        FreetypeNative.FT_Done_Face(_face);
    }
}