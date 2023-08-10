namespace Pie.Windowing.Events;

/// <summary>
/// Contains all supported types of window event.
/// </summary>
public enum WindowEventType
{
    /// <summary>
    /// This event was emitted because the window quit even was raised.
    /// </summary>
    Quit,
    
    /// <summary>
    /// This event was emitted because the window was resized.
    /// </summary>
    Resize,
    
    /// <summary>
    /// This event was emitted because a key was pressed.
    /// </summary>
    KeyDown,
    
    /// <summary>
    /// This event was emitted because a key was released.
    /// </summary>
    KeyUp,
    
    /// <summary>
    /// This event was emitted because a pressed key is repeating.
    /// </summary>
    KeyRepeat,
    
    /// <summary>
    /// This event was emitted because text was inputted to the system.
    /// </summary>
    TextInput,
    
    /// <summary>
    /// This event was emitted because the mouse was moved.
    /// </summary>
    MouseMove,
    
    /// <summary>
    /// This event was emitted because a mouse button was pressed.
    /// </summary>
    MouseButtonDown,
    
    /// <summary>
    /// This event was emitted because a mouse button was released.
    /// </summary>
    MouseButtonUp,
    
    /// <summary>
    /// This event was emitted because the mouse scroll wheel was moved.
    /// </summary>
    MouseScroll
}