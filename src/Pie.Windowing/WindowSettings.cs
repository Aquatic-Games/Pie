using System.Drawing;

namespace Pie.Windowing;

public struct WindowSettings
{
    public Size Size;

    public string Title;

    public WindowBorder Border;

    public bool EventDriven;

    public Icon[] Icons;

    public bool StartVisible;

    public int? StartingMonitor;

    public WindowSettings()
    {
        Size = new Size(1280, 720);
        Title = "Pie Window";
        Border = WindowBorder.Fixed;
        EventDriven = false;
        Icons = null;
        StartVisible = true;
        StartingMonitor = null;
    }
}