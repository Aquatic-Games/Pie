using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Pie.Freetype.Native;
using static Pie.Freetype.Native.FreetypeNative;

namespace Pie.Freetype;

public class FreeType : IDisposable
{
    private FT_Library _library;
    
    public FreeType()
    {
        if (FT_Init_FreeType(out _library) != FT_Error.Ok)
            throw new Exception("Could not initialize freetype.");
    }

    public unsafe Face CreateFace(string path, int initialSize)
    {
        //return CreateFace(File.ReadAllBytes(path), initialSize);
        FT_Face* face;
        fixed (byte* bytes = Encoding.ASCII.GetBytes(path))
            FT_New_Face(_library, (sbyte*) bytes, new FT_Long(0), out face);
        return new Face(face, initialSize);
    }

    public unsafe Face CreateFace(byte[] data, int initialSize)
    {
        FT_Face* face;
        fixed (byte* d = data)
            FT_New_Memory_Face(_library, d, new FT_Long(data.Length), new FT_Long(0), out face);
        return new Face(face, initialSize);
    }

    public void Dispose()
    {
        if (FT_Done_FreeType(_library) != FT_Error.Ok)
            throw new Exception("An error occured during disposal.");
    }
}