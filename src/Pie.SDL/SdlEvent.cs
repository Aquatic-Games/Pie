using System.Runtime.InteropServices;

namespace Pie.SDL;

[StructLayout(LayoutKind.Explicit, Size = 56)]
public unsafe struct SdlEvent
{
    [FieldOffset(0)] public uint Type;

    [FieldOffset(0)] public SdlWindowEvent Window;

    [FieldOffset(0)] public SdlKeyboardEvent Keyboard;

    [FieldOffset(0)] public SdlTextInputEvent Text;
}