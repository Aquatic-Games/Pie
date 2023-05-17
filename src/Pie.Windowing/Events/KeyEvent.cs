namespace Pie.Windowing.Events;

public struct KeyEvent : IWindowEvent
{
    public uint Scancode;
    public Key Key;

    public KeyEvent(WindowEventType eventType, uint scancode, Key key)
    {
        EventType = eventType;
        Scancode = scancode;
        Key = key;
    }

    public WindowEventType EventType { get; }
}