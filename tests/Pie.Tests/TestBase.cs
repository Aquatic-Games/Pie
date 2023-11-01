using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using Pie.Audio;
using Pie.Windowing;
using Pie.Windowing.Events;
using StbImageSharp;
using Point = System.Drawing.Point;
using Window = Pie.Windowing.Window;

namespace Pie.Tests;

public abstract class TestBase : IDisposable
{
    private HashSet<Key> _keysDown;
    
    public Window Window;
    public GraphicsDevice GraphicsDevice;

    protected virtual void Initialize() { }

    protected virtual void Update(double dt) { }

    protected virtual void Draw(double dt) { }

    public void Run(GraphicsApi api)
    {
        Console.WriteLine("Num monitors: " + Monitor.ConnectedMonitors.Length);

        Monitor primary = Monitor.PrimaryMonitor;
        Console.WriteLine("Primary bounds: " + primary.Bounds);
        Console.WriteLine("Primary disp mode: " + primary.CurrentMode + " | AR: " + primary.CurrentMode.AspectRatio + " | AAR: " + primary.CurrentMode.AccurateAspectRatio);
        Console.WriteLine("Primary supported modes:");
        foreach (VideoMode mode in primary.SupportedModes)
            Console.WriteLine(mode + " | AR: " + mode.AspectRatio + " | AAR: " + mode.AccurateAspectRatio);
        
        PieLog.DebugLog += DebugLog;

       Window = new WindowBuilder()
            .Size(1280, 720)
            .Title("Pie Tests")
            .Resizable()
            .Api(api)
            .GraphicsDeviceOptions(new GraphicsDeviceOptions(true))
            .Build(out GraphicsDevice);

        Initialize();

        bool wantsClose = false;
        _keysDown = new HashSet<Key>();

        Stopwatch sw = Stopwatch.StartNew();
        while (!wantsClose)
        {
            MouseDelta = Vector2.Zero;
            
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        wantsClose = true;
                        break;
                    
                    case ResizeEvent resize:
                        ResizeWindow(new Size(resize.Width, resize.Height));
                        break;
                    
                    case KeyEvent key:
                        switch (key.EventType)
                        {
                            case WindowEventType.KeyDown:
                                _keysDown.Add(key.Key);
                                break;
                            case WindowEventType.KeyUp:
                                _keysDown.Remove(key.Key);
                                break;
                        }

                        break;
                    
                    case MouseMoveEvent move:
                        MouseDelta += new Vector2(move.DeltaX, move.DeltaY);
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

    public Vector2 MouseDelta;
    
    public bool IsKeyDown(Key key) => _keysDown.Contains(key);

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

    public virtual void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}