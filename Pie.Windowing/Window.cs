using System;
using System.Drawing;
using Silk.NET.GLFW;

namespace Pie.Windowing;

public unsafe partial class Window : IDisposable
{
    private Glfw _glfw;
    private WindowHandle* _handle;

    private string _title;

    private Window(Glfw glfw, WindowHandle* handle, WindowSettings settings)
    {
        _glfw = glfw;
        _handle = handle;
        _title = settings.Title;
        EventDriven = settings.EventDriven;
        SetupCallbacks();
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
        get => _title;
        set
        {
            _title = value;
            _glfw.SetWindowTitle(_handle, value);
        }
    }

    public void ProcessEvents()
    {
        if (EventDriven)
            _glfw.WaitEvents();
        else
            _glfw.PollEvents();
    }

    public void CenterWindow()
    {
        Monitor* monitor = _glfw.GetPrimaryMonitor();
        VideoMode* mode = _glfw.GetVideoMode(monitor);
        _glfw.GetMonitorPos(monitor, out int x, out int y);
        Size size = Size;
        _glfw.SetWindowPos(_handle, x + mode->Width / 2 - size.Width / 2, y + mode->Height / 2 - size.Height / 2);
    }

    public static Window CreateWithGraphicsDevice(WindowSettings settings, GraphicsApi api, out GraphicsDevice device)
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

        switch (api)
        {
            case GraphicsApi.OpenGl33:
                device = GraphicsDevice.CreateOpenGL33(new GlfwContext(glfw, handle), settings.Size, false);
                break;
            case GraphicsApi.D3D11:
                device = GraphicsDevice.CreateD3D11(new GlfwNativeWindow(glfw, handle).Win32!.Value.Hwnd, settings.Size, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
        
        glfw.ShowWindow(handle);
        return new Window(glfw, handle, settings);
    }

    public static Window CreateWithGraphicsDevice(WindowSettings settings, out GraphicsDevice device)
    {
        return CreateWithGraphicsDevice(settings, GraphicsDevice.GetBestApiForPlatform(), out device);
    }

    public void Dispose()
    {
        _glfw.Terminate();
        _glfw.Dispose();
    }
}