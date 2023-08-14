using System.Drawing;

namespace Pie.Windowing.Events;

/// <summary>
/// Contains information about a window resize event.
/// </summary>
public struct ResizeEvent : IWindowEvent
{
    /// <summary>
    /// The window's new width, in pixels.
    /// </summary>
    public readonly int Width;
    
    /// <summary>
    /// The window's new height, in pixels.
    /// </summary>
    public readonly int Height;

    /// <summary>
    /// Create a new <see cref="ResizeEvent"/>.
    /// </summary>
    /// <param name="width">The window's new width, in pixels.</param>
    /// <param name="height">The window's new height, in pixels.</param>
    public ResizeEvent(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    /// <inheritdoc />
    public WindowEventType EventType => WindowEventType.Resize;
}