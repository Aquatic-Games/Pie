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

    public GraphicsDeviceOptions GraphicsDeviceOptions;

    public GraphicsApi WindowApi;

    public WindowBuilder()
    {
        WindowSize = new Size(1280, 720);
        WindowPosition = null;
        WindowTitle = Assembly.GetEntryAssembly()?.GetName().Name ?? "Pie Window";
        WindowResizable = false;
        GraphicsDeviceOptions = new GraphicsDeviceOptions(false);
        WindowApi = GraphicsDevice.GetBestApiForPlatform();
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

    public WindowBuilder WithGraphicsDeviceOptions(in GraphicsDeviceOptions options)
    {
        GraphicsDeviceOptions = options;

        return this;
    }

    /// <summary>
    /// Set the <see cref="GraphicsApi"/> that will be used for the <see cref="GraphicsDevice"/>.
    /// </summary>
    /// <param name="api">The <see cref="GraphicsApi"/> to use.</param>
    /// <returns>The window builder.</returns>
    public WindowBuilder WithApi(GraphicsApi api)
    {
        WindowApi = api;

        return this;
    }

    public Window Build()
    {
        return new Window(this);
    }

    public Window Build(out GraphicsDevice device, GraphicsDeviceOptions? options = null)
    {
        Window window = new Window(this);
        device = window.CreateGraphicsDevice(options);
        return window;
    }
}