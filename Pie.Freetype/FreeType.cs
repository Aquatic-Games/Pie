using System;
using System.IO;
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

    public Face CreateFace(string path)
    {
        return CreateFace(File.ReadAllBytes(path));
    }

    public unsafe Face CreateFace(byte[] data)
    {
        FT_Face* face;
        fixed (void* d = data)
            FT_New_Memory_Face(_library, (byte*) d, new FT_Long(data.Length), new FT_Long(0), out face);
        return new Face(face);
    }

    public void Dispose()
    {
        if (FT_Done_FreeType(_library) != FT_Error.Ok)
            throw new Exception("An error occured during disposal.");
    }
}