namespace Pie.SDL;

public unsafe struct SdlDisplayMode
{
    public uint Format;
    public int W;
    public int H;
    public int RefreshRate;
    public void* DriverData;
}