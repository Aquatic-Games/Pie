using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Pie.Audio;
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
        Console.WriteLine("Num monitors: " + Monitor.ConnectedMonitors.Length);

        Monitor primary = Monitor.PrimaryMonitor;
        Console.WriteLine("Primary bounds: " + primary.Bounds);
        Console.WriteLine("Primary disp mode: " + primary.CurrentMode + " | AR: " + primary.CurrentMode.AspectRatio + " | AAR: " + primary.CurrentMode.AccurateAspectRatio);
        Console.WriteLine("Primary supported modes:");
        foreach (VideoMode mode in primary.SupportedModes)
            Console.WriteLine(mode + " | AR: " + mode.AspectRatio + " | AAR: " + mode.AccurateAspectRatio);
        
        PieLog.DebugLog += DebugLog;

        //ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("/home/skye/Pictures/pie_1f967.png"), ColorComponents.RedGreenBlueAlpha);
       // Icon icon = new Icon((uint) result.Width, (uint) result.Height, result.Data);

       Window = new WindowBuilder()
            .Size(1280, 720)
            //.FullscreenMode(FullscreenMode.ExclusiveFullscreen)
            .Title("A test with SDL!")
            //.Icon(icon)
            .Resizable()
            //.Hidden()
            //.Borderless()
            .Api(api)
            .GraphicsDeviceOptions(new GraphicsDeviceOptions(true))
            .Build(out GraphicsDevice);

        Initialize();

        bool wantsClose = false;
        
        uint numFrames = 0;
        double currentTime = 0.0;

        Stopwatch sw = Stopwatch.StartNew();
        while (!wantsClose)
        {
            while (Window.PollEvent(out IWindowEvent evnt))
            {
                switch (evnt.EventType)
                {
                    case WindowEventType.Quit:
                        wantsClose = true;
                        break;
                    case WindowEventType.Resize:
                        ResizeEvent resizeEvent = (ResizeEvent) evnt;
                        ResizeWindow(new Size(resizeEvent.Width, resizeEvent.Height));
                        break;
                    case WindowEventType.KeyDown:
                    case WindowEventType.KeyUp:
                    case WindowEventType.KeyRepeat:
                        KeyEvent ke = (KeyEvent) evnt;
                        Console.WriteLine(ke.EventType + ": " + ke.Key + "(" + ke.Scancode + ")");

                        if (ke.EventType == WindowEventType.KeyDown && ke.Key == Key.Space)
                            Window.Minimize();
                        
                        if (ke.EventType == WindowEventType.KeyDown && ke.Key == Key.Escape)
                            wantsClose = true;

                        if (ke.EventType == WindowEventType.KeyDown && ke.Key == Key.F11)
                        {
                            FullscreenMode currentMode = Window.FullscreenMode;
                            Console.WriteLine(currentMode);
                            Window.FullscreenMode = currentMode != FullscreenMode.Windowed
                                ? FullscreenMode.Windowed
                                : FullscreenMode.BorderlessFullscreen;
                        }
                        break;
                    case WindowEventType.TextInput:
                        TextInputEvent text = (TextInputEvent) evnt;
                        
                        Console.WriteLine(text.Text);
                        break;
                    
                    case WindowEventType.MouseMove:
                        MouseMoveEvent moveEvent = (MouseMoveEvent) evnt;
                        Console.WriteLine(
                            $"Mouse X: {moveEvent.MouseX}, Y: {moveEvent.MouseY}, Delta X: {moveEvent.DeltaX}, Delta Y: {moveEvent.DeltaY}");
                        break;
                    
                    case WindowEventType.MouseButtonDown:
                    case WindowEventType.MouseButtonUp:
                        MouseButtonEvent buttonEvent = (MouseButtonEvent) evnt;
                        
                        Console.WriteLine($"{buttonEvent.EventType}: {buttonEvent.Button}");
                        break;
                    
                    case WindowEventType.MouseScroll:
                        MouseScrollEvent scrollEvent = (MouseScrollEvent) evnt;
                        
                        Console.WriteLine($"Scroll X: {scrollEvent.X}, Y: {scrollEvent.Y}");
                        break;
                }
            }

            numFrames++;
            
            double dt = sw.Elapsed.TotalSeconds;
            currentTime += dt;

            if (currentTime >= 1.0)
            {
                currentTime = currentTime - 1.0;
                
                Console.WriteLine(numFrames);
                numFrames = 0;
            }

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

    public virtual void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
}