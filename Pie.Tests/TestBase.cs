using System;
using System.Diagnostics;
using System.Drawing;
using Pie.Windowing;

namespace Pie.Tests;

public abstract class TestBase : IDisposable
{
    public Window Window;
    public GraphicsDevice GraphicsDevice;

    protected virtual void Initialize() { }

    protected virtual void Update(double dt) { }

    protected virtual void Draw(double dt) { }

    public void Run(WindowSettings settings, GraphicsApi api)
    {
        PieLog.DebugLog += (type, message) => Console.WriteLine($"[{type}] {message}");
        
        Window = Window.CreateWithGraphicsDevice(settings, api, out GraphicsDevice);
        Window.Resize += WindowOnResize;

        Initialize();
        
        Stopwatch sw = Stopwatch.StartNew();
        while (!Window.ShouldClose)
        {
            Window.ProcessEvents();

            double dt = sw.Elapsed.TotalSeconds;
            
            Update(dt);
            Draw(dt);
            
            sw.Restart();
            
            GraphicsDevice.Present(1);
        }
    }

    protected virtual void WindowOnResize(Size size)
    {
        GraphicsDevice.ResizeSwapchain(size);
        GraphicsDevice.Viewport = new Rectangle(Point.Empty, size);
        Console.WriteLine($"resize: {size}");
    }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}