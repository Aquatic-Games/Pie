using System;
using System.Runtime.InteropServices;
using Pie.Text.Native;

namespace Pie.Text;

public unsafe class Face : IDisposable
{
    private FT_Face* _face;
    private byte* _faceData;
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

    public readonly string Family;
    public readonly string Style;

    public readonly CharacterCollection Characters;
    public readonly FaceFlags Flags;

    internal Face(FT_Face* face, byte* data, int initialSize, FaceFlags flags)
    {
        _face = face;
        _faceData = data;
        
        Characters = new CharacterCollection(_face, flags);
        Size = initialSize;
        
        Family = Marshal.PtrToStringAnsi((IntPtr) face->FamilyName);
        Style = Marshal.PtrToStringAnsi((IntPtr) face->StyleName);
        
        Flags = flags;
    }
    
    public void Dispose()
    {
        FreetypeNative.FT_Done_Face(_face);
        if (_faceData != null)
            NativeMemory.Free(_faceData);
    }
}