using System;
using System.Drawing;
using System.Reflection;

namespace Pie.Windowing;

public class WindowBuilder
{
    public Size WindowSize;

    public Point? WindowPosition;

    public string WindowTitle;

    public bool WindowResizable;

    public bool WindowBorderless;

    public Icon? WindowIcon;

    public GraphicsDeviceOptions DeviceOptions;

    public GraphicsApi WindowApi;

    public FullscreenMode WindowFullscreenMode;

    public WindowBuilder()
    {
        WindowSize = new Size(1280, 720);
        WindowPosition = null;
        WindowTitle = Assembly.GetEntryAssembly()?.GetName().Name ?? "Pie Window";
        WindowResizable = false;
        WindowBorderless = false;
        WindowIcon = null;
        DeviceOptions = new GraphicsDeviceOptions(false);
        WindowApi = GraphicsDevice.GetBestApiForPlatform();
        WindowFullscreenMode = Windowing.FullscreenMode.Windowed;
    }

    public WindowBuilder Size(int width, int height)
    {
        WindowSize = new Size(width, height);

        return this;
    }

    public WindowBuilder Title(string title)
    {
        WindowTitle = title;

        return this;
    }

    public WindowBuilder Position(int x, int y)
    {
        WindowPosition = new Point(x, y);

        return this;
    }

    public WindowBuilder Resizable()
    {
        WindowResizable = true;

        return this;
    }

    public WindowBuilder Borderless()
    {
        WindowBorderless = true;

        return this;
    }

    public WindowBuilder Icon(in Icon icon)
    {
        WindowIcon = icon;

        return this;
    }

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

    public WindowBuilder FullscreenMode(FullscreenMode fullscreenMode)
    {
        WindowFullscreenMode = fullscreenMode;

        return this;
    }

    public Window Build()
    {
        return new Window(this);
    }

    public Window Build(out GraphicsDevice device)
    {
        Window window = new Window(this);
        device = window.CreateGraphicsDevice(DeviceOptions);
        return window;
    }
}