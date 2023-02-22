using System;
using System.Diagnostics;
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
        Window = Window.CreateWithGraphicsDevice(settings, api, out GraphicsDevice);

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

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}