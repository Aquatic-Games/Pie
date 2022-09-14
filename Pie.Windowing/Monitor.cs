using System.Collections.Generic;
using System.Drawing;
using Silk.NET.GLFW;
using GLFWmonitor = Silk.NET.GLFW.Monitor;
using GLFWvidmode = Silk.NET.GLFW.VideoMode;

namespace Pie.Windowing;

public struct Monitor
{
    public readonly Rectangle Bounds;

    public readonly Point Location;

    public readonly VideoMode VideoMode;

    public readonly VideoMode[] AvailableModes;

    private Monitor(Rectangle bounds, VideoMode mode, VideoMode[] availableModes)
    {
        Bounds = bounds;
        Location = bounds.Location;
        VideoMode = mode;
        AvailableModes = availableModes;
    }

    private static Monitor[] _monitors;
    
    public static Monitor PrimaryMonitor => _monitors[0];
    
    public static Monitor[] ConnectedMonitors => _monitors;

    internal static unsafe void DetectMonitors(Glfw glfw)
    {
        GLFWmonitor** monitors = glfw.GetMonitors(out int mCount);
        _monitors = new Monitor[mCount];
        for (int m = 0; m < mCount; m++)
        {
            GLFWmonitor* monitor = monitors[m];
            glfw.GetMonitorPos(monitor, out int x, out int y);
            GLFWvidmode* mode = glfw.GetVideoMode(monitor);
            Rectangle bounds = new Rectangle(new Point(x, y), new Size(mode->Width, mode->Height));
            VideoMode vMode = new VideoMode(bounds.Size, mode->RefreshRate);
            List<VideoMode> availableModes = new List<VideoMode>();
            GLFWvidmode* modes = glfw.GetVideoModes(monitor, out int vCount);
            for (int v = 0; v < vCount; v++)
                availableModes.Add(new VideoMode(new Size(modes[v].Width, modes[v].Height), modes[v].RefreshRate));
            _monitors[m] = new Monitor(bounds, vMode, availableModes.ToArray());
        }
    }
}