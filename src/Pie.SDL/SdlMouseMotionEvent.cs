namespace Pie.SDL;

public struct SdlMouseMotionEvent
{
    public uint Type;
    public uint Timestamp;
    public uint WindowID;
    public uint Which;
    public uint State;
    public int X;
    public int Y;
    public int XRel;
    public int YRel;
}