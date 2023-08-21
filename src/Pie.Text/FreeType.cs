using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Pie.Text.Native;
using static Pie.Text.Native.FreetypeNative;

namespace Pie.Text;

public class FreeType : IDisposable
{
    private FT_Library _library;
    
    public FreeType()
    {
        if (FT_Init_FreeType(out _library) != FT_Error.Ok)
            throw new Exception("Could not initialize freetype.");
    }

    public unsafe Face CreateFace(string path, FaceFlags flags = FaceFlags.Antialiased | FaceFlags.RgbaConvert)
    {
        //return CreateFace(File.ReadAllBytes(path), initialSize);
        FT_Face* face;
        fixed (byte* bytes = Encoding.ASCII.GetBytes(path))
            FT_New_Face(_library, (sbyte*) bytes, new FT_Long(0), out face);
        return new Face(face, null, flags);
    }

    public unsafe Face CreateFace(byte[] data, FaceFlags flags = FaceFlags.Antialiased | FaceFlags.RgbaConvert)
    {
        // The small footnote in freetype says:
        // "You must not deallocate the memory before calling FT_Done_Face."
        // Finally knowing this... (uugghhh this has caused years of pain)
        // Allocate & copy the data into a separate buffer which is kept alive while the face is alive.
        byte* pData = (byte*) NativeMemory.Alloc((nuint) data.Length);
        fixed (byte* dPtr = data)
            Unsafe.CopyBlock(pData, dPtr, (uint) data.Length);
        
        FT_Face* face;
        FT_New_Memory_Face(_library, pData, new FT_Long(data.Length), new FT_Long(0), out face);
        return new Face(face, pData, flags);
    }

    public void Dispose()
    {
        if (FT_Done_FreeType(_library) != FT_Error.Ok)
            throw new Exception("An error occured during disposal.");
    }
}