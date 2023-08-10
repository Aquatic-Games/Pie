namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a key event. This event is emitted when a key is pressed, released, or repeats.
/// </summary>
public struct KeyEvent : IWindowEvent
{
    /// <summary>
    /// The raw scan code of the key.
    /// </summary>
    public readonly uint Scancode;
    
    /// <summary>
    /// The key that has been pressed, released, or is repeating.
    /// </summary>
    public readonly Key Key;

    /// <summary>
    /// Creates a new <see cref="KeyEvent"/>.
    /// </summary>
    /// <param name="eventType">The key event type.</param>
    /// <param name="scancode">The raw scan code of the key.</param>
    /// <param name="key">The key that has been pressed, released, or is repeating.</param>
    public KeyEvent(WindowEventType eventType, uint scancode, Key key)
    {
        EventType = eventType;
        Scancode = scancode;
        Key = key;
    }

    /// <inheritdoc />
    public WindowEventType EventType { get; }
}