using System;
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

        while (!_window.ShouldClose)
        {
            _window.ProcessEvents();
            
            GraphicsDevice.Present(1);
        }
    }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        _window.Dispose();
    }
}