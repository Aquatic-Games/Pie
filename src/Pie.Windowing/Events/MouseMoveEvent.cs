namespace Pie.Windowing.Events;

public struct MouseMoveEvent : IWindowEvent
{
    public readonly int MouseX;
    public readonly int MouseY;
    public readonly int DeltaX;
    public readonly int DeltaY;

    public MouseMoveEvent(int x, int y, int xrel, int yrel)
    {
        MouseX = x;
        MouseY = y;
        DeltaX = xrel;
        DeltaY = yrel;
    }
    
    public WindowEventType EventType => WindowEventType.MouseMove;
}