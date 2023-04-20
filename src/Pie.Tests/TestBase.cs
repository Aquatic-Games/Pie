using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Pie.Windowing;
using Pie.Windowing.Events;
using StbImageSharp;
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

        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/pie_1f967.png"), ColorComponents.RedGreenBlueAlpha);
        Icon icon = new Icon((uint) result.Width, (uint) result.Height, result.Data);
        
        Window = new WindowBuilder()
            .Size(800, 480)
            .Title("A test with SDL!")
            .Icon(icon)
            .Resizable()
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
                    case QuitEvent:
                        wantsClose = true;
                        break;
                    case ResizeEvent rEvent:
                        ResizeWindow(rEvent.Size);
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

    protected virtual void ResizeWindow(Size size)
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