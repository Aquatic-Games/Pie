namespace Pie.Windowing.Events;

public struct KeyEvent : IWindowEvent
{
    public readonly uint Scancode;
    public readonly Key Key;

    public KeyEvent(WindowEventType eventType, uint scancode, Key key)
    {
        EventType = eventType;
        Scancode = scancode;
        Key = key;
    }

    public WindowEventType EventType { get; }
}