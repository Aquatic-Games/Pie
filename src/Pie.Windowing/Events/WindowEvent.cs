namespace Pie.Windowing.Events;

/// <summary>
/// The base interface for a window event.
/// </summary>
public interface IWindowEvent
{
    /// <summary>
    /// The <see cref="WindowEventType"/> of this event.
    /// </summary>
    public WindowEventType EventType { get; }
}