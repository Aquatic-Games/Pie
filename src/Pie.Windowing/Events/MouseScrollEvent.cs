namespace Pie.Windowing.Events;

public struct MouseScrollEvent : IWindowEvent
{
    public readonly float X;
    public readonly float Y;

    public MouseScrollEvent(float x, float y)
    {
        X = x;
        Y = y;
    }

    public WindowEventType EventType => WindowEventType.MouseScroll;
}