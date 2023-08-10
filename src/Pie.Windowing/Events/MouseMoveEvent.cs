namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a mouse move event.
/// </summary>
public struct MouseMoveEvent : IWindowEvent
{
    /// <summary>
    /// The current mouse X position, in pixels, relative to the window.
    /// </summary>
    public readonly int MouseX;
    
    /// <summary>
    /// The current mouse Y position, in pixels, relative to the window.
    /// </summary>
    public readonly int MouseY;
    
    /// <summary>
    /// The delta mouse X position.
    /// </summary>
    public readonly int DeltaX;
    
    /// <summary>
    /// The delta mouse X position.
    /// </summary>
    public readonly int DeltaY;

    /// <summary>
    /// Create a new <see cref="MouseMoveEvent"/>.
    /// </summary>
    /// <param name="x">The current mouse X position, in pixels, relative to the window.</param>
    /// <param name="y">The current mouse Y position, in pixels, relative to the window.</param>
    /// <param name="xrel">The delta mouse X position.</param>
    /// <param name="yrel">The delta mouse X position.</param>
    public MouseMoveEvent(int x, int y, int xrel, int yrel)
    {
        MouseX = x;
        MouseY = y;
        DeltaX = xrel;
        DeltaY = yrel;
    }
    
    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.MouseMove;
}