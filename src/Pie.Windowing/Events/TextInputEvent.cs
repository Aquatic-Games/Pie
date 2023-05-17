namespace Pie.Windowing.Events;

public struct TextInputEvent : IWindowEvent
{
    public string Text;

    public TextInputEvent(string text)
    {
        Text = text;
    }

    public WindowEventType EventType => WindowEventType.TextInput;
}