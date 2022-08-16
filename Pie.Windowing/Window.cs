using System;
using System.Drawing;
using Silk.NET.GLFW;

namespace Pie.Windowing;

public unsafe partial class Window : IDisposable
{
    private Glfw _glfw;
    private WindowHandle* _handle;
    private WindowSettings _settings;
    private GraphicsApi _api;

    private InputState _inputState;

    private Window(Glfw glfw, WindowHandle* handle, WindowSettings settings, GraphicsApi api)
    {
        _glfw = glfw;
        _handle = handle;
        _settings = settings;
        _api = api;
        EventDriven = settings.EventDriven;
        SetupCallbacks();
        _inputState = new InputState(this, handle, glfw);
    }

    public bool EventDriven;

    public Size Size
    {
        get
        {
            _glfw.GetWindowSize(_handle, out int width, out int height);
            return new Size(width, height);
        }
        set
        {
            _glfw.SetWindowSize(_handle, value.Width, value.Height);
            CenterWindow();
        }
    }

    public bool ShouldClose
    {
        get => _glfw.WindowShouldClose(_handle);
        set => _glfw.SetWindowShouldClose(_handle, value);
    }

    public string Title
    {
        get => _settings.Title;
        set
        {
            _settings.Title = value;
            _glfw.SetWindowTitle(_handle, value);
        }
    }

    public InputState ProcessEvents()
    {
        _inputState.Update(_handle, _glfw);
        if (EventDriven)
            _glfw.WaitEvents();
        else
            _glfw.PollEvents();
        return _inputState;
    }

    public void CenterWindow()
    {
        Monitor* monitor = _glfw.GetPrimaryMonitor();
        VideoMode* mode = _glfw.GetVideoMode(monitor);
        _glfw.GetMonitorPos(monitor, out int x, out int y);
        Size size = Size;
        _glfw.SetWindowPos(_handle, x + mode->Width / 2 - size.Width / 2, y + mode->Height / 2 - size.Height / 2);
    }

    public static Window CreateWindow(WindowSettings settings, GraphicsApi api)
    {
        Glfw glfw = Glfw.GetApi();
        if (!glfw.Init())
            throw new PieException("GLFW failed to initialize.");
        
        glfw.WindowHint(WindowHintBool.Visible, false);
        glfw.WindowHint(WindowHintBool.Resizable, settings.Resizable);
        switch (api)
        {
            case GraphicsApi.OpenGl33:
                glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
                glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
                glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
                glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
                break;
            case GraphicsApi.D3D11:
                glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }

        WindowHandle* handle = glfw.CreateWindow(settings.Size.Width, settings.Size.Height, settings.Title, null, null);
        if (handle == null)
        {
            glfw.Terminate();
            throw new PieException(
                $"The window could not be created. This most likely means the chosen graphics API ({api.ToFriendlyString()}) is not supported on the given platform/hardware.");
        }

        Monitor* monitor = glfw.GetPrimaryMonitor();
        VideoMode* mode = glfw.GetVideoMode(monitor);
        glfw.GetMonitorPos(monitor, out int x, out int y);
        glfw.SetWindowPos(handle, x + mode->Width / 2 - settings.Size.Width / 2, y + mode->Height / 2 - settings.Size.Height / 2);
        
        glfw.MakeContextCurrent(handle);
        glfw.ShowWindow(handle);
        return new Window(glfw, handle, settings, api);
    }

    public static Window CreateWindow(WindowSettings settings)
    {
        return CreateWindow(settings, GraphicsDevice.GetBestApiForPlatform());
    }

    public static Window CreateWithGraphicsDevice(WindowSettings settings, GraphicsApi api, out GraphicsDevice device,
        GraphicsDeviceCreationFlags flags = GraphicsDeviceCreationFlags.None)
    {
        Window window = CreateWindow(settings, api);
        
        switch (api)
        {
            case GraphicsApi.OpenGl33:
                device = GraphicsDevice.CreateOpenGL33(new GlfwContext(window._glfw, window._handle), settings.Size, flags);
                break;
            case GraphicsApi.D3D11:
                device = GraphicsDevice.CreateD3D11(new GlfwNativeWindow(window._glfw, window._handle).Win32!.Value.Hwnd, settings.Size, flags);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }

        return window;
    }

    public static Window CreateWithGraphicsDevice(WindowSettings settings, out GraphicsDevice device)
    {
        return CreateWithGraphicsDevice(settings, GraphicsDevice.GetBestApiForPlatform(), out device);
    }
    
    public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceCreationFlags flags = GraphicsDeviceCreationFlags.None)
    {
        _glfw.MakeContextCurrent(_handle);
        return _api switch
        {
            GraphicsApi.OpenGl33 => GraphicsDevice.CreateOpenGL33(new GlfwContext(_glfw, _handle), _settings.Size,
                flags),
            GraphicsApi.D3D11 => GraphicsDevice.CreateD3D11(new GlfwNativeWindow(_glfw, _handle).Win32!.Value.Hwnd,
                _settings.Size, flags),
            _ => throw new ArgumentOutOfRangeException(nameof(_api), _api, null)
        };
    }

    public void Dispose()
    {
        _glfw.Terminate();
        _glfw.Dispose();
    }
}