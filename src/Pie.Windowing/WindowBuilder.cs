using System.Drawing;
using System.Reflection;

namespace Pie.Windowing;

/// <summary>
/// Various parameters used to construct a <see cref="Window"/>.
/// </summary>
public class WindowBuilder
{
    /// <summary>
    /// The size/resolution of the window, in screen coordinates.
    /// </summary>
    public Size WindowSize;

    /// <summary>
    /// The position of the window. If null, the window will be centered on the primary monitor.
    /// </summary>
    public Point? WindowPosition;

    /// <summary>
    /// The title of the window.
    /// </summary>
    public string WindowTitle;

    /// <summary>
    /// If true, the window should be resizable.
    /// </summary>
    public bool WindowResizable;

    /// <summary>
    /// If true, the window should not have a border.
    /// </summary>
    public bool WindowBorderless;

    /// <summary>
    /// If true, the window will be hidden once built.
    /// </summary>
    public bool WindowHidden;

    /// <summary>
    /// The window icon it should use. This should be an RGBA formatted bitmap. If null, the default window manager icon
    /// is used.
    /// </summary>
    public Icon? WindowIcon;

    /// <summary>
    /// The <see cref="Pie.GraphicsDeviceOptions"/> to use when calling <see cref="Build(out GraphicsDevice)"/>
    /// </summary>
    public GraphicsDeviceOptions DeviceOptions;

    /// <summary>
    /// The <see cref="GraphicsApi"/> to use.
    /// </summary>
    public GraphicsApi WindowApi;

    /// <summary>
    /// The <see cref="Pie.Windowing.FullscreenMode"/> to use.
    /// </summary>
    public FullscreenMode WindowFullscreenMode;

    /// <summary>
    /// Create a new <see cref="WindowBuilder"/> with the default settings.
    /// </summary>
    public WindowBuilder()
    {
        WindowSize = new Size(1280, 720);
        WindowPosition = null;
        WindowTitle = Assembly.GetEntryAssembly()?.GetName().Name ?? "Pie Window";
        WindowResizable = false;
        WindowBorderless = false;
        WindowHidden = false;
        WindowIcon = null;
        DeviceOptions = new GraphicsDeviceOptions(false);
        WindowApi = GraphicsDevice.GetBestApiForPlatform();
        WindowFullscreenMode = Windowing.FullscreenMode.Windowed;
    }

    /// <summary>
    /// The size/resolution of the window, in screen coordinates.
    /// </summary>
    public WindowBuilder Size(int width, int height)
    {
        WindowSize = new Size(width, height);

        return this;
    }

    /// <summary>
    /// The title of the window.
    /// </summary>
    public WindowBuilder Title(string title)
    {
        WindowTitle = title;

        return this;
    }

    /// <summary>
    /// The position of the window.
    /// </summary>
    public WindowBuilder Position(int x, int y)
    {
        WindowPosition = new Point(x, y);

        return this;
    }

    /// <summary>
    /// Hint that the window should be resizable.
    /// </summary>
    public WindowBuilder Resizable()
    {
        WindowResizable = true;

        return this;
    }

    /// <summary>
    /// Hint that the window should not have a border.
    /// </summary>
    public WindowBuilder Borderless()
    {
        WindowBorderless = true;

        return this;
    }

    /// <summary>
    /// Hint that the window should not be visible once built.
    /// </summary>
    public WindowBuilder Hidden()
    {
        WindowHidden = true;

        return this;
    }

    /// <summary>
    /// Set the window icon. This must be an RGBA formatted bitmap.
    /// </summary>
    /// <param name="icon">The icon to use.</param>
    public WindowBuilder Icon(in Icon icon)
    {
        WindowIcon = icon;

        return this;
    }

    /// <summary>
    /// The <see cref="Pie.GraphicsDeviceOptions"/> to use when calling <see cref="Build(out GraphicsDevice)"/>
    /// </summary>
    public WindowBuilder GraphicsDeviceOptions(in GraphicsDeviceOptions options)
    {
        DeviceOptions = options;

        return this;
    }

    /// <summary>
    /// Set the <see cref="GraphicsApi"/> that will be used for the <see cref="GraphicsDevice"/>.
    /// </summary>
    /// <param name="api">The <see cref="GraphicsApi"/> to use.</param>
    /// <returns>The window builder.</returns>
    public WindowBuilder Api(GraphicsApi api)
    {
        WindowApi = api;

        return this;
    }

    /// <summary>
    /// Set the window fullscreen mode.
    /// </summary>
    /// <param name="fullscreenMode">The <see cref="Pie.Windowing.FullscreenMode"/> to use.</param>
    public WindowBuilder FullscreenMode(FullscreenMode fullscreenMode)
    {
        WindowFullscreenMode = fullscreenMode;

        return this;
    }

    /// <summary>
    /// Builds the window with the set options.
    /// </summary>
    /// <returns>The built window.</returns>
    public Window Build()
    {
        return new Window(this);
    }

    /// <summary>
    /// Builds the window with the set options, as well as creating a graphics device from the given <see cref="DeviceOptions"/>.
    /// </summary>
    /// <param name="device">The created <see cref="GraphicsDevice"/>.</param>
    /// <returns>The built window.</returns>
    public Window Build(out GraphicsDevice device)
    {
        Window window = new Window(this);
        device = window.CreateGraphicsDevice(DeviceOptions);
        return window;
    }
}