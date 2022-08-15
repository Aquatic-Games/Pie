using System.Drawing;

namespace Pie.Windowing;

public struct WindowSettings
{
    public Size Size;

    public string Title;

    public bool Resizable;

    public bool EventDriven;

    public WindowSettings()
    {
        Size = new Size(1280, 720);
        Title = "Pie Window";
        Resizable = false;
        EventDriven = false;
    }
}