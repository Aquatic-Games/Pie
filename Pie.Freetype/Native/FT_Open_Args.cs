using System.Runtime.InteropServices;
using System;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Open_Args
{
    public FT_UInt Flags;
    public FT_Byte* MemoryBase;
    public FT_Long MemorySize;
    public FT_String* PathName;
    public FT_Stream* Stream;
    public IntPtr Driver;
    public FT_Int NumParams;
    public FT_Parameter* Params;
}