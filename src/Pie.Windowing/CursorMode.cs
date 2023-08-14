namespace Pie.Windowing;

/// <summary>
/// Represents various cursor modes supported by a <see cref="Window"/>.
/// </summary>
public enum CursorMode
{
    /// <summary>
    /// The cursor is visible and free to move anywhere.
    /// </summary>
    Visible,
    
    /// <summary>
    /// The cursor is hidden when on top of this window, however is free to move anywhere.
    /// </summary>
    Hidden,
    
    /// <summary>
    /// The cursor is grabbed by the window, and is locked inside of its bounds. However, the cursor is still free to
    /// move anywhere inside of this window.
    /// </summary>
    Grabbed,
    
    /// <summary>
    /// The cursor is hidden and locked to the center of the window.
    /// </summary>
    Locked
}