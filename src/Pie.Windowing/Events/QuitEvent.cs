namespace Pie.Windowing.Events;

public class QuitEvent : IWindowEvent
{
    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.Quit;
}