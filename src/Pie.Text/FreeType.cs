using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using FreeTypeSharp;
using static FreeTypeSharp.FT;

namespace Pie.Text;

public unsafe class FreeType : IDisposable
{
    private FT_LibraryRec_* _library;
    
    public FreeType()
    {
        fixed (FT_LibraryRec_** library = &_library)
        {
            if (FT_Init_FreeType(library) != FT_Error.FT_Err_Ok)
                throw new Exception("Could not initialize freetype.");
        }
    }

    public unsafe Face CreateFace(string path, FaceFlags flags = FaceFlags.Antialiased | FaceFlags.RgbaConvert)
    {
        FT_FaceRec_* face;
        fixed (byte* bytes = Encoding.ASCII.GetBytes(path))
            FT_New_Face(_library, bytes, (nint) 0, &face);
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
        
        FT_FaceRec_* face;
        FT_New_Memory_Face(_library, pData, (nint) data.Length, (nint) 0, &face);
        return new Face(face, pData, flags);
    }

    public void Dispose()
    {
        if (FT_Done_FreeType(_library) != FT_Error.FT_Err_Ok)
            throw new Exception("An error occured during disposal.");
    }
}