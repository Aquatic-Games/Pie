using System;
using System.Drawing;
using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;
using Monitor = Silk.NET.GLFW.Monitor;

namespace Pie.Windowing;

public unsafe partial class Window : IDisposable
{
    private WindowHandle* _window;
    private WindowSettings _settings;
    private Glfw _glfw;

    public IntPtr Handle => (IntPtr) _window;
    
    public IGLContext GlContext { get; private set; }

    public Size Size
    {
        get
        {
            _glfw.GetWindowSize(_window, out int width, out int height);
            return new Size(width, height);
        }
        set
        {
            _glfw.SetWindowSize(_window, value.Width, value.Height);
        }
    }

    public bool ShouldClose
    {
        get => _glfw.WindowShouldClose(_window);
        set => _glfw.SetWindowShouldClose(_window, value);
    }

    public Window(WindowSettings settings)
    {
        _settings = settings;
    }
    
    public void Run()
    {
        _glfw = Glfw.GetApi();

        if (!_glfw.Init())
            throw new Exception("GLFW failed to initialize.");
        
        _glfw.WindowHint(WindowHintBool.Visible, false);
        _glfw.WindowHint(WindowHintBool.Resizable, _settings.Resizable);
        _glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
        _glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
        _glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        _glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);

        _window = _glfw.CreateWindow(_settings.Size.Width, _settings.Size.Height, _settings.Title, null, null);
        if (_window == null)
        {
            Dispose();
            throw new Exception(
                "Window failed to create - it's likely the selected API is not supported on this system.");
        }
        
        // TODO: Add D3D support.
        _glfw.MakeContextCurrent(_window);
        GlContext = new GlfwContext(_glfw, _window);

        InitializeInput();

        Monitor* primary = _glfw.GetPrimaryMonitor();
        VideoMode* mode = _glfw.GetVideoMode(primary);
        _glfw.GetMonitorPos(primary, out int x, out int y);
        _glfw.SetWindowPos(_window, x + mode->Width / 2 - _settings.Size.Width / 2,
            y + mode->Height / 2 - _settings.Size.Height / 2);
        
        _glfw.ShowWindow(_window);
    }

    public void Update()
    {
        if (_settings.EventDriven)
            _glfw.WaitEvents();
        else
            _glfw.PollEvents();
    }

    public void Dispose()
    {
        _glfw.Terminate();
        _glfw.Dispose();
    }
}