using System.Runtime.InteropServices;

namespace Pie.Text.Native;

[StructLayout(LayoutKind.Explicit)]
// Union type
public unsafe struct FT_StreamDesc
{
    [FieldOffset(0)] public CLong Value;

    [FieldOffset(0)] public void* Pointer;
}