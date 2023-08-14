namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a text input event.
/// </summary>
public struct TextInputEvent : IWindowEvent
{
    /// <summary>
    /// The text contained in this event.
    /// </summary>
    public readonly string Text;

    /// <summary>
    /// Creates a new <see cref="TextInputEvent"/>.
    /// </summary>
    /// <param name="text">The text contained in this event.</param>
    public TextInputEvent(string text)
    {
        Text = text;
    }

    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.TextInput;
}