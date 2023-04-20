using System.Drawing;

namespace Pie.Windowing.Events;

public struct ResizeEvent : IWindowEvent
{
    public WindowEventType EventType => WindowEventType.Resize;

    public Size Size;

    public ResizeEvent(Size size)
    {
        Size = size;
    }
}