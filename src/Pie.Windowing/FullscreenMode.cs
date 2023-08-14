namespace Pie.Windowing;

/// <summary>
/// Represents supported fullscreen modes of a <see cref="Window"/>.
/// </summary>
public enum FullscreenMode
{
    /// <summary>
    /// The window is not fullscreen.
    /// </summary>
    Windowed,
    
    /// <summary>
    /// The window is exclusively fullscreen.
    /// </summary>
    ExclusiveFullscreen,
    
    /// <summary>
    /// The window is borderless fullscreen. Sometimes this is called a "fullscreen window".
    /// </summary>
    BorderlessFullscreen
}