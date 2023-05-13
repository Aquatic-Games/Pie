using System.Runtime.InteropServices;

namespace Pie.Windowing.SdlNative;

[StructLayout(LayoutKind.Sequential)]
public struct SdlKeyboardEvent
{
    public uint Type;
    public uint Timestamp;
    public uint WindowID;
    public byte State;
    public byte Repeat;
    public byte Padding2;
    public byte Padding3;
    
}