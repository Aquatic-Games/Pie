using System;
using System.Drawing;
using System.Numerics;
using Pie.Windowing;

namespace Pie.Tests;

public class Main : IDisposable
{
    private Window _window;
    public GraphicsDevice GraphicsDevice;

    public void Run()
    {
        _window = Window.CreateWithGraphicsDevice(new WindowSettings(), GraphicsApi.Vulkan, out GraphicsDevice,
            new GraphicsDeviceOptions(true));

        int i = 0;

        while (!_window.ShouldClose)
        {
            i = (i + 1) % 255;
            Vector4 color = new Vector4(i / 255f, 0, (255 - 1) / 255f, 1);
            
            _window.ProcessEvents();
            GraphicsDevice.Clear(color);

            GraphicsDevice.Present(1);
        }
    }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        _window.Dispose();
    }
}