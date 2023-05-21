using System.Drawing;

namespace Pie.Windowing.Events;

public struct ResizeEvent : IWindowEvent
{
    public readonly int Width;
    public readonly int Height;

    public ResizeEvent(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public WindowEventType EventType => WindowEventType.Resize;
}