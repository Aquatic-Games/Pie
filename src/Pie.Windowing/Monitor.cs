using System.Drawing;
using Pie.SDL;

namespace Pie.Windowing;

/// <summary>
/// Contains information about monitors connected to the system.
/// </summary>
public struct Monitor
{
    /// <summary>
    /// The bounds of the monitor, in pixels.
    /// </summary>
    public readonly Rectangle Bounds;

    /// <summary>
    /// The <see cref="VideoMode"/> of the monitor.
    /// </summary>
    public readonly VideoMode CurrentMode;

    /// <summary>
    /// All <see cref="VideoMode"/>s supported by the monitor.
    /// </summary>
    public readonly VideoMode[] SupportedModes;

    /// <summary>
    /// Create a new monitor. Yes. One will just suddenly appear on your desk.
    /// </summary>
    /// <param name="bounds">The bounds of the monitor, in pixels.</param>
    /// <param name="currentMode">The <see cref="VideoMode"/> of the monitor.</param>
    /// <param name="supportedModes">All <see cref="VideoMode"/>s supported by the monitor.</param>
    public Monitor(Rectangle bounds, VideoMode currentMode, VideoMode[] supportedModes)
    {
        Bounds = bounds;
        CurrentMode = currentMode;
        SupportedModes = supportedModes;
    }

    static unsafe Monitor()
    {
        // Init in case this is called before a Pie window is created.
        if (Sdl.Init(Sdl.InitVideo) < 0)
            throw new PieException("Failed to initialize SDL: " + Sdl.GetErrorS());

        int numDisplays = Sdl.GetNumVideoDisplays();
        if (numDisplays < 0)
            throw new PieException("Failed to get number of displays: " + Sdl.GetErrorS());

        ConnectedMonitors = new Monitor[numDisplays];
        
        for (int d = 0; d < numDisplays; d++)
        {
            SdlRect bounds;
            if (Sdl.GetDisplayBounds(d, &bounds) < 0)
                throw new PieException("Failed to get display bounds: " + Sdl.GetErrorS());

            int numDisplayModes = Sdl.GetNumDisplayModes(d);
            if (numDisplayModes < 0)
                throw new PieException("Failed to get display modes: " + Sdl.GetErrorS());
            
            SdlDisplayMode currentMode;
            if (Sdl.GetDesktopDisplayMode(d, &currentMode) < 0)
                throw new PieException("Failed to get current display mode: " + Sdl.GetErrorS());
            
            VideoMode current = new VideoMode(new Size(currentMode.W, currentMode.H), currentMode.RefreshRate);

            VideoMode[] modes = new VideoMode[numDisplayModes];

            for (int m = 0; m < numDisplayModes; m++)
            {
                SdlDisplayMode mode;
                Sdl.GetDisplayMode(d, m, &mode);

                modes[m] = new VideoMode(new Size(mode.W, mode.H), mode.RefreshRate);
            }

            ConnectedMonitors[d] = new Monitor(new Rectangle(bounds.X, bounds.Y, bounds.W, bounds.H), current, modes);
        }
    }

    /// <summary>
    /// Contains all monitors currently connected to the system.
    /// </summary>
    public static readonly Monitor[] ConnectedMonitors;

    /// <summary>
    /// The primary monitor connected to the system.
    /// </summary>
    public static Monitor PrimaryMonitor => ConnectedMonitors[0];
}