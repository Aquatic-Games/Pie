using System.Runtime.InteropServices;

namespace Pie.SDL;

[StructLayout(LayoutKind.Sequential)]
public struct SdlWindowEvent
{
    public uint Type;
    public uint Timestamp;
    public uint WindowId;

    public byte Event;

    public byte Padding1;
    public byte Padding2;
    public byte Padding3;

    public int Data1;
    public int Data2;
}