namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a mouse scroll event.
/// </summary>
public struct MouseScrollEvent : IWindowEvent
{
    /// <summary>
    /// The delta X position of the scroll wheel.
    /// </summary>
    public readonly float X;
    
    /// <summary>
    /// The delta Y position of the scroll wheel.
    /// </summary>
    public readonly float Y;

    /// <summary>
    /// Create a new <see cref="MouseScrollEvent"/>.
    /// </summary>
    /// <param name="x">The delta X position of the scroll wheel.</param>
    /// <param name="y">The delta Y position of the scroll wheel.</param>
    public MouseScrollEvent(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.MouseScroll;
}