namespace Pie.Windowing.Events;

public struct MouseButtonEvent : IWindowEvent
{
    public readonly MouseButton Button;
    
    public MouseButtonEvent(WindowEventType eventType, MouseButton button)
    {
        EventType = eventType;
        Button = button;
    }
    
    public WindowEventType EventType { get; }
}