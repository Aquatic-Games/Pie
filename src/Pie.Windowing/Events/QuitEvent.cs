namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a window quit event.
/// </summary>
public struct QuitEvent : IWindowEvent
{
    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.Quit;
}