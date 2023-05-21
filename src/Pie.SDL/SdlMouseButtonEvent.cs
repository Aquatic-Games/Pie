namespace Pie.SDL;

public struct SdlMouseButtonEvent
{
    public uint Type;
    public uint Timestamp;
    public uint WindowID;
    public uint Which;
    public byte Button;
    public byte State;
    public byte Clicks;
    public int X;
    public int Y;
}