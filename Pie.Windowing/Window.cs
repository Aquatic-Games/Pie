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
        set => _glfw.SetWindowSize(_handle, value.Width, value.Height);
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

    public MouseState MouseState
    {
        get
        {
            CursorModeValue state = (CursorModeValue) _glfw.GetInputMode(_handle, CursorStateAttribute.Cursor);
            return state switch
            {
                CursorModeValue.CursorNormal => MouseState.Visible,
                CursorModeValue.CursorHidden => MouseState.Hidden,
                CursorModeValue.CursorDisabled => MouseState.Locked,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        set
        {
            CursorModeValue val = value switch
            {
                MouseState.Visible => CursorModeValue.CursorNormal,
                MouseState.Hidden => CursorModeValue.CursorHidden,
                MouseState.Locked => CursorModeValue.CursorDisabled,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            _glfw.SetInputMode(_handle, CursorStateAttribute.Cursor, val);
        }
    }

    private WindowBorder _border;
    public WindowBorder Border
    {
        get => _border;
        set
        {
            _border = value;
            switch (value)
            {
                case WindowBorder.Fixed:
                    _glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Decorated, true);
                    _glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Resizable, false);
                    break;
                case WindowBorder.Borderless:
                    _glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Decorated, false);
                    _glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Resizable, false);
                    break;
                case WindowBorder.Resizable:
                    _glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Decorated, true);
                    _glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Resizable, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    public bool Visible
    {
        get => _glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Visible);
        set
        {
            if (value)
                _glfw.ShowWindow(_handle);
            else
                _glfw.HideWindow(_handle);
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

    public void Center()
    {
        Rectangle bounds = Monitor.PrimaryMonitor.Bounds;
        Size size = Size;
        _glfw.SetWindowPos(_handle, bounds.X + bounds.Width / 2 - size.Width / 2, bounds.Y + bounds.Height / 2 - size.Height / 2);
    }

    public void Minimize()
    {
        _glfw.IconifyWindow(_handle);
    }

    public void Maximize()
    {
        _glfw.MaximizeWindow(_handle);
    }

    public void Restore()
    {
        _glfw.RestoreWindow(_handle);
    }

    public static Window CreateWindow(WindowSettings settings, GraphicsApi api)
    {
        Glfw glfw = Glfw.GetApi();
        if (!glfw.Init())
            throw new PieException("GLFW failed to initialize.");
        
        glfw.WindowHint(WindowHintBool.Visible, false);
        switch (settings.Border)
        {
            case WindowBorder.Fixed:
                glfw.WindowHint(WindowHintBool.Decorated, true);
                glfw.WindowHint(WindowHintBool.Resizable, false);
                break;
            case WindowBorder.Borderless:
                glfw.WindowHint(WindowHintBool.Decorated, false);
                glfw.WindowHint(WindowHintBool.Resizable, false);
                break;
            case WindowBorder.Resizable:
                glfw.WindowHint(WindowHintBool.Decorated, true);
                glfw.WindowHint(WindowHintBool.Resizable, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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

        Monitor.DetectMonitors(glfw);

        Rectangle bounds = default;
        if (settings.StartingMonitor == null)
        {
            glfw.GetCursorPos(handle, out double mx, out double my);
            foreach (Monitor monitor in Monitor.ConnectedMonitors)
            {
                bounds = monitor.Bounds;
                if (bounds.Contains((int) mx, (int) my))
                    break;
            }
        }
        else
            bounds = Monitor.ConnectedMonitors[settings.StartingMonitor.Value].Bounds;

        glfw.SetWindowPos(handle, bounds.X + bounds.Width / 2 - settings.Size.Width / 2,
            bounds.Y + bounds.Height / 2 - settings.Size.Height / 2);

        if (settings.Icons != null)
        {
            Image[] images = new Image[settings.Icons.Length];
            for (int i = 0; i < images.Length; i++)
            {
                ref Icon icon = ref settings.Icons[i];
                fixed (byte* pixels = icon.Data)
                {
                    images[i] = new Image()
                    {
                        Width = (int) icon.Width,
                        Height = (int) icon.Height,
                        Pixels = pixels
                    };
                }
            }
            
            fixed (Image* imgs = images)
                glfw.SetWindowIcon(handle, settings.Icons.Length, imgs);
        }
        
        glfw.MakeContextCurrent(handle);
        glfw.ShowWindow(handle);

        return new Window(glfw, handle, settings, api);
    }

    public static Window CreateWindow(WindowSettings settings)
    {
        return CreateWindow(settings, GraphicsDevice.GetBestApiForPlatform());
    }

    public static Window CreateWithGraphicsDevice(WindowSettings settings, GraphicsApi api, out GraphicsDevice device,
        GraphicsDeviceOptions options = default)
    {
        Window window = CreateWindow(settings, api);
        
        switch (api)
        {
            case GraphicsApi.OpenGl33:
                device = GraphicsDevice.CreateOpenGL33(new GlfwContext(window._glfw, window._handle), settings.Size, options);
                break;
            case GraphicsApi.D3D11:
                device = GraphicsDevice.CreateD3D11(new GlfwNativeWindow(window._glfw, window._handle).Win32!.Value.Hwnd, settings.Size, options);
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
    
    public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions options = default)
    {
        _glfw.MakeContextCurrent(_handle);
        return _api switch
        {
            GraphicsApi.OpenGl33 => GraphicsDevice.CreateOpenGL33(new GlfwContext(_glfw, _handle), _settings.Size,
                options),
            GraphicsApi.D3D11 => GraphicsDevice.CreateD3D11(new GlfwNativeWindow(_glfw, _handle).Win32!.Value.Hwnd,
                _settings.Size, options),
            _ => throw new ArgumentOutOfRangeException(nameof(_api), _api, null)
        };
    }

    public void Dispose()
    {
        _glfw.Terminate();
        _glfw.Dispose();
    }
}