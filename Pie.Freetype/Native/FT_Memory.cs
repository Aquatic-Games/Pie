using System.Runtime.InteropServices;

namespace Pie.Freetype.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Memory
{
    public void* User;

    public delegate*<FT_Memory*, CLong, void*> Alloc;

    public delegate*<FT_Memory*, void*, void> Free;

    public delegate*<FT_Memory*, CLong, CLong, void*, void*> Realloc;
}