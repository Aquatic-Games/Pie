using System.Runtime.InteropServices;

namespace Pie.Windowing.SdlNative;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SdlSysWmInfo
{
    private fixed byte _version[3];
    
    private uint _syswmtype;

    public WindowInfo Info;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct WindowInfo
{
    [FieldOffset(0)] public Win32Info Win;

    [FieldOffset(0)] private fixed byte _padding[64];
}

[StructLayout(LayoutKind.Sequential)]
public struct Win32Info
{
    public nint Window;
    public nint Hdc;
    public nint Hinstance;
}