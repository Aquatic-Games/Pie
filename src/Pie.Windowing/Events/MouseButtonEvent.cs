namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a mouse button event. This is emitted when a button is pressed or released.
/// </summary>
public struct MouseButtonEvent : IWindowEvent
{
    /// <summary>
    /// The button that has been pressed or released.
    /// </summary>
    public readonly MouseButton Button;
    
    /// <summary>
    /// Create a new <see cref="MouseButtonEvent"/>.
    /// </summary>
    /// <param name="eventType">The mouse button event type.</param>
    /// <param name="button">The button that has been pressed or released.</param>
    public MouseButtonEvent(WindowEventType eventType, MouseButton button)
    {
        EventType = eventType;
        Button = button;
    }
    
    /// <inheritdoc />
    public WindowEventType EventType { get; }
}