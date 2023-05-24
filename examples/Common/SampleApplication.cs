using System;
using System.Diagnostics;
using System.Drawing;
using Pie;
using Pie.Audio;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace Common;

public abstract class SampleApplication : IDisposable
{
    private string _title;
    private Size _size;
    private bool _wantsClose;

    public Window Window;
    public GraphicsDevice GraphicsDevice;
    public AudioDevice AudioDevice;
    
    protected SampleApplication(Size size, string title)
    {
        _size = size;
        _title = title;
        _wantsClose = false;
        
        PieLog.DebugLog += Log;
    }

    protected virtual void Initialize() { }

    protected virtual void Update(double dt) { }

    protected virtual void Draw(double dt) { }

    public void Run()
    {
        Log(LogType.Debug, "Creating window and device.");
        Window = new WindowBuilder()
            .Size(_size.Width, _size.Height)
            .Title(_title)
            .Resizable()
            .GraphicsDeviceOptions(new GraphicsDeviceOptions() { Debug = true })
            .Build(out GraphicsDevice);
        
        Log(LogType.Debug, "Creating audio device.");
        AudioDevice = new AudioDevice(48000, 16);
        
        Log(LogType.Debug, "Initializing application.");
        Initialize();
        
        Log(LogType.Debug, "Entering render loop.");
        
        Stopwatch sw = Stopwatch.StartNew();

        while (!_wantsClose)
        {
            Input.NewFrame();
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        _wantsClose = true;
                        break;
                    case ResizeEvent resize:
                        Log(LogType.Warning, $"New size {resize.Width}x{resize.Height}");
                        GraphicsDevice.ResizeSwapchain(new Size(resize.Width, resize.Height));
                        GraphicsDevice.Viewport = new Rectangle(0, 0, resize.Width, resize.Height);
                        break;
                    
                    case KeyEvent key:
                        switch (key.EventType)
                        {
                            case WindowEventType.KeyDown:
                                Input.AddKeyDown(key);
                                break;
                            case WindowEventType.KeyUp:
                                Input.AddKeyUp(key);
                                break;
                        }

                        break;
                    
                    case MouseMoveEvent mouseMove:
                        Input.AddMouseMove(mouseMove);
                        break;
                }
            }

            double delta = sw.Elapsed.TotalSeconds;
            sw.Restart();
            
            Update(delta);
            Draw(delta);
            
            GraphicsDevice.Present(1);
        }
    }

    public void Close()
    {
        _wantsClose = true;
    }

    public virtual void Dispose()
    {
        GraphicsDevice.Dispose();
        Window.Dispose();
    }
    
    public static void Log(LogType logtype, string message)
    {
        if (logtype == LogType.Critical)
            throw new Exception("Critical error! " + message);

        // Pad to ensure each log type has the same alignment
        Console.WriteLine($"[{logtype}] ".PadRight(10) + message);
    }
}
