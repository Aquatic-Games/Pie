namespace Pie.SDL;

public struct SdlMouseWheelEvent
{
    public uint Type;
    public uint Timestamp;
    public uint WindowID;
    public uint Which;
    public int X;
    public int Y;
    public uint Direction;
    public float PreciseX;
    public float PreciseY;
}