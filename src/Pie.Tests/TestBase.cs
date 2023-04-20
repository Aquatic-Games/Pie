using System;
using System.Diagnostics;
using System.Drawing;
using Pie.Windowing;
using Pie.Windowing.Events;
using Point = System.Drawing.Point;
using Window = Pie.Windowing.Window;

namespace Pie.Tests;

public abstract class TestBase : IDisposable
{
    public Window Window;
    public GraphicsDevice GraphicsDevice;

    protected virtual void Initialize() { }

    protected virtual void Update(double dt) { }

    protected virtual void Draw(double dt) { }

    public void Run(GraphicsApi api)
    {
        PieLog.DebugLog += DebugLog;

        Window = new WindowBuilder()
            .WithApi(api)
            .Build(out GraphicsDevice);

        Initialize();

        bool wantsClose = false;
        
        Stopwatch sw = Stopwatch.StartNew();
        while (!wantsClose)
        {
            while (Window.PollEvent(out IWindowEvent evnt))
            {
                switch (evnt)
                {
                    case QuitEvent qEvent:
                        wantsClose = true;
                        break;
                }
            }

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
    
    private void DebugLog(LogType logtype, string message)
    {
        if (logtype == LogType.Critical)
            throw new Exception(message);
        
        Console.WriteLine($"[{logtype}] " + message);
    }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}