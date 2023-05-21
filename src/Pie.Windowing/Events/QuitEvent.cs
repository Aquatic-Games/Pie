namespace Pie.Windowing.Events;

public struct QuitEvent : IWindowEvent
{
    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.Quit;
}