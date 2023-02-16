using System;
using System.Drawing;
using Pie.Vulkan;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;
using Image = Silk.NET.GLFW.Image;

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

    /// <summary>
    /// Whether or not this window is event driven. If true, <see cref="ProcessEvents"/> will block the thread until
    /// an event occurs (such as mouse movement, key press, etc.)
    /// </summary>
    public bool EventDriven;

    /// <summary>
    /// The size, in pixels, of this window.
    /// </summary>
    public Size Size
    {
        get
        {
            _glfw.GetWindowSize(_handle, out int width, out int height);
            return new Size(width, height);
        }
        set => _glfw.SetWindowSize(_handle, value.Width, value.Height);
    }

    /// <summary>
    /// Whether or not the window should close. Use this in your application loop.
    /// </summary>
    public bool ShouldClose
    {
        get => _glfw.WindowShouldClose(_handle);
        set => _glfw.SetWindowShouldClose(_handle, value);
    }

    /// <summary>
    /// The title of this window, as displayed in the title bar.
    /// </summary>
    public string Title
    {
        get => _settings.Title;
        set
        {
            _settings.Title = value;
            _glfw.SetWindowTitle(_handle, value);
        }
    }

    /// <summary>
    /// The current <see cref="Pie.Windowing.MouseState"/> of this window.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid enum value is provided.</exception>
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
    /// <summary>
    /// The border of this window. Set as <see cref="WindowBorder.Resizable"/> to make this window resizable.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid enum value is provided.</exception>
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

    /// <summary>
    /// Whether or not this window is focused.
    /// </summary>
    public bool Focused => _glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Focused);

    /// <summary>
    /// Whether or not this window is visible.
    /// </summary>
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

    /// <summary>
    /// Whether or not this window is full screen. If set true, the window will become fullscreen at its current <see cref="Size"/>,
    /// and will use the refresh rate of the current monitor. Use <see cref="SetFullscreen"/> for more control.
    /// </summary>
    public bool Fullscreen
    {
        get => _glfw.GetWindowMonitor(_handle) != null;
        set
        {
            Monitor primary = Monitor.PrimaryMonitor;
            Size size = Size;
            _glfw.SetWindowMonitor(_handle, value ? _glfw.GetPrimaryMonitor() : null, 0, 0, size.Width, size.Height, primary.VideoMode.RefreshRate);
            if (!value)
                Center();
        }
    }

    /// <summary>
    /// Process events such as keyboard and mouse input, resize events, and handling the close button being clicked.
    /// </summary>
    /// <returns></returns>
    public InputState ProcessEvents()
    {
        _inputState.Update(_handle, _glfw);
        if (EventDriven)
            _glfw.WaitEvents();
        else
            _glfw.PollEvents();
        return _inputState;
    }

    /// <summary>
    /// Center this window on the primary monitor.
    /// </summary>
    public void Center()
    {
        Rectangle bounds = Monitor.PrimaryMonitor.Bounds;
        Size size = Size;
        _glfw.SetWindowPos(_handle, bounds.X + bounds.Width / 2 - size.Width / 2, bounds.Y + bounds.Height / 2 - size.Height / 2);
    }

    /// <summary>
    /// Minimize this window to the taskbar.
    /// </summary>
    public void Minimize()
    {
        _glfw.IconifyWindow(_handle);
    }

    /// <summary>
    /// Maximize this window.
    /// </summary>
    public void Maximize()
    {
        _glfw.MaximizeWindow(_handle);
    }

    /// <summary>
    /// Restores this window if it has been minimized.
    /// </summary>
    public void Restore()
    {
        _glfw.RestoreWindow(_handle);
    }

    /// <summary>
    /// Set this window's full screen mode.
    /// </summary>
    /// <param name="fullscreen">Whether or not the window is fullscreen.</param>
    /// <param name="resolution">The new resolution of the window.</param>
    /// <param name="refreshRate">The refresh rate, if any. Set as -1 to use the monitor's refresh rate.</param>
    /// <param name="monitorIndex">The monitor index. 0 is the primary monitor.</param>
    public void SetFullscreen(bool fullscreen, Size resolution, int refreshRate = -1, int monitorIndex = 0)
    {
        _glfw.SetWindowMonitor(_handle, fullscreen ? _glfw.GetMonitors(out _)[monitorIndex] : null, 0, 0, resolution.Width, resolution.Height, refreshRate);
        if (!fullscreen)
            Center();
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
            case GraphicsApi.OpenGL:
                glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
                glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
                glfw.WindowHint(WindowHintInt.ContextVersionMajor, 4);
                glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
                glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
                break;
            case GraphicsApi.D3D11:
                glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
                break;
            case GraphicsApi.Vulkan:
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

        // Set the window position to 0 as on Windows the window starts up in a seemingly random position
        glfw.SetWindowPos(handle, 0, 0);
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
        if (settings.StartVisible)
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
            case GraphicsApi.OpenGL:
                device = GraphicsDevice.CreateOpenGL33(new GlfwContext(window._glfw, window._handle), window.Size, options);
                break;
            case GraphicsApi.D3D11:
                device = GraphicsDevice.CreateD3D11(new GlfwNativeWindow(window._glfw, window._handle).Win32!.Value.Hwnd, window.Size, options);
                break;
            case GraphicsApi.Vulkan:
                byte** extensionPtr = window._glfw.GetRequiredInstanceExtensions(out uint count);
                string[] extensions = SilkMarshal.PtrToStringArray((IntPtr) extensionPtr, (int) count);
                VkHelper.InitVulkan(extensions, options.Debug);

                VkNonDispatchableHandle surface = new VkNonDispatchableHandle();
                Result result = (Result) window._glfw.CreateWindowSurface(VkHelper.Instance.ToHandle(), window._handle, null, &surface);
                if (result != Result.Success)
                    throw new PieException("Failed to create window surface: " + result);
                SurfaceKHR khrSurface = surface.ToSurface();
                
                device = GraphicsDevice.CreateVulkan((nint) khrSurface.Handle, window.Size, options);
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
            GraphicsApi.OpenGL => GraphicsDevice.CreateOpenGL33(new GlfwContext(_glfw, _handle), _settings.Size,
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

    private static void CenterWindowInternal(Glfw glfw, WindowHandle* handle, Size size)
    {
        
    }
}