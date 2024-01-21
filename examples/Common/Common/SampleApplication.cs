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

    private int _numFrames;
    private double _fpsDelta;

    private int _fps;
    public int Fps => _fps;
    
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

    protected virtual void Resize(Size size) { }

    public void Run()
    {
        Log(LogType.Debug, "Checking for \"DEMO_FORCE_API\" environment variable...");
        string forceApi = Environment.GetEnvironmentVariable("DEMO_FORCE_API");

        GraphicsApi api;
        if (forceApi == null)
            api = GraphicsDevice.GetBestApiForPlatform();
        else
        {
            Log(LogType.Debug, $"Attempting to use API \"{forceApi}\".");
            if (!Enum.TryParse(forceApi, true, out api))
            {
                Log(LogType.Debug, "Attempt failed. Reverting to default API.");
                api = GraphicsDevice.GetBestApiForPlatform();
            }
        }
        
        Log(LogType.Info, $"Using {api.ToFriendlyString()} graphics API.");
        
        Log(LogType.Debug, "Creating window and device.");
        Window = new WindowBuilder()
            .Size(_size.Width, _size.Height)
            .Title(_title)
            .Resizable()
            .Api(api)
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

            bool focused = Window.Focused;
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        _wantsClose = true;
                        break;
                    case ResizeEvent resize:
                        Log(LogType.Info, $"New size {resize.Width}x{resize.Height}");
                        Size size = new Size(resize.Width, resize.Height);
                        GraphicsDevice.ResizeSwapchain(size);
                        GraphicsDevice.Viewport = new Rectangle(0, 0, resize.Width, resize.Height);
                        Resize(size);
                        break;
                    
                    case KeyEvent key:
                        if (!focused)
                            break;
                        
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
                        if (!focused)
                            break;
                        
                        Input.AddMouseMove(mouseMove);
                        break;
                }
            }

            double delta = sw.Elapsed.TotalSeconds;
            sw.Restart();

            _fpsDelta += delta;
            _numFrames++;
            if (_fpsDelta >= 1.0)
            {
                _fpsDelta -= 1.0;
                _fps = _numFrames;
                Window.Title = _title + $" | {GraphicsDevice.Api} - {_fps} FPS";
                _numFrames = 0;
            }

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
