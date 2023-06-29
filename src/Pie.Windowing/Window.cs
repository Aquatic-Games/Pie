using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Pie.OpenGL;
using Pie.Windowing.Events;
using Pie.SDL;

namespace Pie.Windowing;

public sealed unsafe class Window : IDisposable
{
    private void* _window;
    
    private void* _glContext;

    private GraphicsApi _api;

    /// <summary>
    /// The SDL window handle.
    /// </summary>
    public IntPtr Handle => (IntPtr) _window;

    /// <summary>
    /// The size, in <b>screen coordinates</b>, of the window.
    /// </summary>
    public Size Size
    {
        get
        {
            int width, height;
            Sdl.GetWindowSize(_window, &width, &height);
            return new Size(width, height);
        }

        set => Sdl.SetWindowSize(_window, value.Width, value.Height);
    }

    /// <summary>
    /// Get the size of the window <b>in pixels</b>. NOTE: This is <b>NOT</b> the same as <see cref="Size"/>, and you
    /// should use this property when performing actions such as resizing the swapchain.
    /// </summary>
    public Size FramebufferSize
    {
        get
        {
            int width, height;
            Sdl.GetWindowSizeInPixels(_window, &width, &height);
            return new Size(width, height);
        }
    }

    /// <summary>
    /// Get or set the window position.
    /// </summary>
    public Point Position
    {
        get
        {
            int x, y;
            Sdl.GetWindowPosition(_window, &x, &y);
            return new Point(x, y);
        }
        set => Sdl.SetWindowPosition(_window, value.X, value.Y);
    }

    /// <summary>
    /// Get or set the title of the window.
    /// </summary>
    public string Title
    {
        get
        {
            sbyte* title = Sdl.GetWindowTitle(_window);
            return Marshal.PtrToStringAnsi((IntPtr) title);
        }
        set
        {
            fixed (byte* title = Encoding.UTF8.GetBytes(value))
                Sdl.SetWindowTitle(_window, (sbyte*) title);
        }
    }

    public FullscreenMode FullscreenMode
    {
        get
        {
            SdlWindowFlags flags = Sdl.GetWindowFlags(_window);

            if ((flags & SdlWindowFlags.FullscreenDesktop) == SdlWindowFlags.FullscreenDesktop)
                return FullscreenMode.BorderlessFullscreen;
            if ((flags & SdlWindowFlags.Fullscreen) == SdlWindowFlags.Fullscreen)
                return FullscreenMode.ExclusiveFullscreen;

            return FullscreenMode.Windowed;
        }
        set
        {
            SdlWindowFlags flags = value switch
            {
                FullscreenMode.Windowed => 0,
                FullscreenMode.ExclusiveFullscreen => SdlWindowFlags.Fullscreen,
                FullscreenMode.BorderlessFullscreen => SdlWindowFlags.FullscreenDesktop,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            Sdl.SetWindowFullscreen(_window, flags);
        }
    }

    public CursorMode CursorMode
    {
        get
        {
            bool visible = Sdl.ShowCursor(Sdl.Query) == Sdl.Enable;
            bool grabbed = Sdl.GetWindowGrab(_window);
            bool relative = Sdl.GetRelativeMouseMode();

            if (!grabbed && !relative)
                return visible ? CursorMode.Visible : CursorMode.Hidden;

            return relative ? CursorMode.Locked : CursorMode.Grabbed;
        }
        set
        {
            switch (value)
            {
                case CursorMode.Visible:
                    Sdl.SetRelativeMouseMode(false);
                    Sdl.SetWindowGrab(_window, false);
                    Sdl.ShowCursor(Sdl.Enable);
                    break;
                case CursorMode.Hidden:
                    Sdl.SetRelativeMouseMode(false);
                    Sdl.SetWindowGrab(_window, false);
                    Sdl.ShowCursor(Sdl.Disable);
                    break;
                case CursorMode.Grabbed:
                    Sdl.SetRelativeMouseMode(false);
                    Sdl.SetWindowGrab(_window, true);
                    Sdl.ShowCursor(Sdl.Enable);
                    break;
                case CursorMode.Locked:
                    Sdl.SetRelativeMouseMode(true);
                    Sdl.SetWindowGrab(_window, true);
                    Sdl.ShowCursor(Sdl.Disable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    public bool Resizable
    {
        get => (Sdl.GetWindowFlags(_window) & SdlWindowFlags.Resizable) == SdlWindowFlags.Resizable;
        set => Sdl.SetWindowResizable(_window, value);
    }

    public bool Borderless
    {
        get => (Sdl.GetWindowFlags(_window) & SdlWindowFlags.Borderless) == SdlWindowFlags.Borderless;
        set => Sdl.SetWindowBordered(_window, !value);
    }

    public bool Visible
    {
        get => (Sdl.GetWindowFlags(_window) & SdlWindowFlags.Shown) == SdlWindowFlags.Shown;
        set
        {
            if (value)
                Sdl.ShowWindow(_window);
            else
                Sdl.HideWindow(_window);
        }
    }

    public bool Focused => (Sdl.GetWindowFlags(_window) & SdlWindowFlags.InputFocus) == SdlWindowFlags.InputFocus;

    internal Window(WindowBuilder builder)
    {
        if (Sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new PieException($"SDL failed to initialize: {Sdl.GetErrorS()}");
        
        // TODO: Disable/make optional.
        // I simply disable this cause I find it annoying during development.
        // I *would* use wayland but it no worky on my 1060 for whatever reason and I am not bothered enough to fix.
        Sdl.SetHint("SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR", "0");

        Point position = builder.WindowPosition ?? new Point((int) Sdl.WindowposCentered, (int) Sdl.WindowposCentered);

        SdlWindowFlags flags = SdlWindowFlags.None;

        if (builder.WindowResizable)
            flags |= SdlWindowFlags.Resizable;
        if (builder.WindowBorderless)
            flags |= SdlWindowFlags.Borderless;
        if (builder.WindowHidden)
            flags |= SdlWindowFlags.Hidden;

        flags |= builder.WindowFullscreenMode switch
        {
            FullscreenMode.Windowed => SdlWindowFlags.None,
            FullscreenMode.ExclusiveFullscreen => SdlWindowFlags.Fullscreen,
            FullscreenMode.BorderlessFullscreen => SdlWindowFlags.FullscreenDesktop,
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (builder.WindowApi)
        {
            case GraphicsApi.OpenGL:
            case GraphicsApi.OpenGLES:
                flags |= SdlWindowFlags.OpenGL;
                Sdl.GLSetAttribute(SdlGlAttr.ContextMajorVersion, 4);
                Sdl.GLSetAttribute(SdlGlAttr.ContextMinorVersion, 3);
                Sdl.GLSetAttribute(SdlGlAttr.ContextProfileMask,
                    builder.WindowApi == GraphicsApi.OpenGLES ? (int) SdlGlProfile.ES : (int) SdlGlProfile.Core);

                (int r, int g, int b, int a, bool srgb, bool fp) bits;
                
                // TODO: Compare behaviour with D3D11 to make sure each combination works.
                // There may still be more of these formats to add into the "unsupported" pile.
                switch (builder.DeviceOptions.ColorBufferFormat)
                {
                    case Format.R8_UNorm:
                    case Format.R8_SNorm:
                    case Format.R8_SInt:
                    case Format.R8_UInt:
                        bits = (8, 0, 0, 0, false, false);
                        break;
                    
                    case Format.R8G8_UNorm:
                    case Format.R8G8_SNorm:
                    case Format.R8G8_SInt:
                    case Format.R8G8_UInt:
                        bits = (8, 8, 0, 0, false, false);
                        break;
                    
                    case Format.R8G8B8A8_UNorm:
                    case Format.R8G8B8A8_SNorm:
                    case Format.R8G8B8A8_SInt:
                    case Format.R8G8B8A8_UInt:
                    case Format.B8G8R8A8_UNorm:
                        bits = (8, 8, 8, 8, false, false);
                        break;
                    
                    case Format.R8G8B8A8_UNorm_SRgb:
                    case Format.B8G8R8A8_UNorm_SRgb:
                        bits = (8, 8, 8, 8, true, false);
                        break;
                    
                    case Format.R16_UNorm:
                    case Format.R16_SNorm:
                    case Format.R16_SInt:
                    case Format.R16_UInt:
                        bits = (16, 0, 0, 0, false, false);
                        break;
                    
                    case Format.R16_Float:
                        bits = (16, 0, 0, 0, false, true);
                        break;
                    
                    case Format.R16G16_UNorm:
                    case Format.R16G16_SNorm:
                    case Format.R16G16_SInt:
                    case Format.R16G16_UInt:
                        bits = (16, 16, 0, 0, false, false);
                        break;
                    
                    case Format.R16G16_Float:
                        bits = (16, 16, 0, 0, false, true);
                        break;
                    
                    case Format.R16G16B16A16_UNorm:
                    case Format.R16G16B16A16_SNorm:
                    case Format.R16G16B16A16_SInt:
                    case Format.R16G16B16A16_UInt:
                        bits = (16, 16, 16, 16, false, false);
                        break;
                    
                    case Format.R16G16B16A16_Float:
                        bits = (16, 16, 16, 16, false, true);
                        break;
                    
                    case Format.R32_SInt:
                    case Format.R32_UInt:
                        bits = (32, 0, 0, 0, false, false);
                        break;
                    
                    case Format.R32_Float:
                        bits = (32, 0, 0, 0, false, true);
                        break;
                    
                    case Format.R32G32_SInt:
                    case Format.R32G32_UInt:
                        bits = (32, 32, 0, 0, false, false);
                        break;
                    
                    case Format.R32G32_Float:
                        bits = (32, 32, 0, 0, false, true);
                        break;
                    
                    case Format.R32G32B32_SInt:
                    case Format.R32G32B32_UInt:
                        bits = (32, 32, 32, 0, false, false);
                        break;
                    
                    case Format.R32G32B32_Float:
                        bits = (32, 32, 32, 0, false, true);
                        break;
                    
                    case Format.R32G32B32A32_SInt:
                    case Format.R32G32B32A32_UInt:
                        bits = (32, 32, 32, 32, false, false);
                        break;
                    
                    case Format.R32G32B32A32_Float:
                        bits = (32, 32, 32, 32, false, true);
                        break;
                    
                    case Format.D24_UNorm_S8_UInt:
                    case Format.D32_Float:
                    case Format.D16_UNorm:
                    case Format.BC1_UNorm:
                    case Format.BC1_UNorm_SRgb:
                    case Format.BC2_UNorm:
                    case Format.BC2_UNorm_SRgb:
                    case Format.BC3_UNorm:
                    case Format.BC3_UNorm_SRgb:
                    case Format.BC4_UNorm:
                    case Format.BC4_SNorm:
                    case Format.BC5_UNorm:
                    case Format.BC5_SNorm:
                    case Format.BC6H_UF16:
                    case Format.BC6H_SF16:
                    case Format.BC7_UNorm:
                    case Format.BC7_UNorm_SRgb:
                        throw new NotSupportedException("The given format cannot be used as a color buffer format.");
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                (int depth, int stencil) depthBits;

                switch (builder.DeviceOptions.DepthStencilBufferFormat)
                {
                    case Format.D24_UNorm_S8_UInt:
                        depthBits = (24, 8);
                        break;
                    
                    case Format.D32_Float:
                        depthBits = (32, 0);
                        break;
                    
                    case Format.D16_UNorm:
                        depthBits = (16, 0);
                        break;
                    
                    case null:
                        depthBits = (0, 0);
                        break;
                    
                    default:
                        throw new NotSupportedException("The given format cannot be used as a depth format.");
                }

                Sdl.GLSetAttribute(SdlGlAttr.RedSize, bits.r);
                Sdl.GLSetAttribute(SdlGlAttr.GreenSize, bits.g);
                Sdl.GLSetAttribute(SdlGlAttr.BlueSize, bits.b);
                Sdl.GLSetAttribute(SdlGlAttr.AlphaSize, bits.a);

                Sdl.GLSetAttribute(SdlGlAttr.DepthSize, depthBits.depth);
                Sdl.GLSetAttribute(SdlGlAttr.StencilSize, depthBits.stencil);
                
                Sdl.GLSetAttribute(SdlGlAttr.FramebufferSrgbCapable, bits.srgb ? 1 : 0);
                Sdl.GLSetAttribute(SdlGlAttr.FloatBuffers, bits.fp ? 1 : 0);
                
                break;
            case GraphicsApi.D3D11:
            case GraphicsApi.Null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        fixed (byte* title = Encoding.UTF8.GetBytes(builder.WindowTitle))
        {
            _window = Sdl.CreateWindow((sbyte*) title, position.X, position.Y, builder.WindowSize.Width,
                builder.WindowSize.Height, (uint) flags);
        }

        if (_window == null)
        {
            Sdl.Quit();
            throw new PieException($"Window failed to create. {Sdl.GetErrorS()}");
        }

        if (builder.WindowIcon != null)
        {
            Icon icon = builder.WindowIcon.Value;
            void* surface;
            fixed (void* ptr = icon.Data)
            {
                // ABGR ?????
                // The hell endianness has SDL been compiled in?
                surface = Sdl.CreateRGBSurfaceWithFormatFrom(ptr, (int) icon.Width, (int) icon.Height, 0,
                    (int) icon.Width * 4, (uint) SdlPixelFormat.ABGR8888);
            }

            Sdl.SetWindowIcon(_window, surface);
        }

        if (builder.WindowApi is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
        {
            _glContext = Sdl.GLCreateContext(_window);
            if (_glContext == null)
                throw new PieException($"Failed to create GL context. {Sdl.GetErrorS()}");

            // Juuust make sure the context is current, even though it should already be.
            if (Sdl.GLMakeCurrent(_window, _glContext) < 0)
                throw new PieException($"Failed to make GL context current. {Sdl.GetErrorS()}");
        }

        _api = builder.WindowApi;
    }

    public void Focus() => Sdl.RaiseWindow(_window);

    public void Center() => Sdl.SetWindowPosition(_window, (int) Sdl.WindowposCentered, (int) Sdl.WindowposCentered);

    public void Maximize() => Sdl.MaximizeWindow(_window);

    public void Minimize() => Sdl.MinimizeWindow(_window);

    public void Restore() => Sdl.RestoreWindow(_window);

    public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions? options = null)
    {
        int width, height;
        
        Sdl.GetWindowSizeInPixels(_window, &width, &height);
        Size size = new Size(width, height);
        
        switch (_api)
        {
            case GraphicsApi.OpenGL:
            case GraphicsApi.OpenGLES:
                return GraphicsDevice.CreateOpenGL(new PieGlContext(Sdl.GLGetProcAddress, i =>
                {
                    Sdl.GLSetSwapInterval(i);
                    Sdl.GLSwapWindow(_window);
                }), size, _api == GraphicsApi.OpenGLES, options ?? new GraphicsDeviceOptions());
            
            case GraphicsApi.D3D11:
                SdlSysWmInfo info = new SdlSysWmInfo();
                Sdl.GetWindowWMInfo(_window, &info);
                return GraphicsDevice.CreateD3D11(info.Info.Win.Window, size,
                    options ?? new GraphicsDeviceOptions());
            
            case GraphicsApi.Null:
                return GraphicsDevice.CreateNull(size, options ?? new GraphicsDeviceOptions());
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool PollEvent(out IWindowEvent @event)
    {
        SdlEvent sdlEvent;
        @event = null;
        if (!Sdl.PollEvent(&sdlEvent))
            return false;

        switch ((SdlEventType) sdlEvent.Type)
        {
            case SdlEventType.Quit:
                @event = new QuitEvent();
                break;
            case SdlEventType.WindowEvent:
                switch ((SdlWindowEventId) sdlEvent.Window.Event)
                {
                    case SdlWindowEventId.Resized:
                        @event = new ResizeEvent(sdlEvent.Window.Data1, sdlEvent.Window.Data2);
                        break;
                    default:
                        // Filter out unrecognized events.
                        return PollEvent(out @event);
                }

                break;
            
            case SdlEventType.KeyDown:
                ref SdlKeyboardEvent kde = ref sdlEvent.Keyboard;

                WindowEventType kdeType = kde.Repeat != 0 ? WindowEventType.KeyRepeat : WindowEventType.KeyDown;

                @event = new KeyEvent(kdeType, kde.ScanCode, SdlHelper.KeycodeToKey(kde.KeyCode));
                break;
            case SdlEventType.KeyUp:
                ref SdlKeyboardEvent kue = ref sdlEvent.Keyboard;

                @event = new KeyEvent(WindowEventType.KeyUp, kue.ScanCode, SdlHelper.KeycodeToKey(kue.KeyCode));
                break;
            
            case SdlEventType.TextInput:
                ref SdlTextInputEvent textEvent = ref sdlEvent.Text;

                fixed (char* text = textEvent.Text)
                    @event = new TextInputEvent(new string(text));

                break;
            
            case SdlEventType.MouseMotion:
                ref SdlMouseMotionEvent motionEvent = ref sdlEvent.Motion;

                @event = new MouseMoveEvent(motionEvent.X, motionEvent.Y, motionEvent.XRel, motionEvent.YRel);

                break;
            
            case SdlEventType.MouseButtonDown:
                ref SdlMouseButtonEvent bdEvent = ref sdlEvent.Button;

                @event = new MouseButtonEvent(WindowEventType.MouseButtonDown, (MouseButton) bdEvent.Button);

                break;
            
            case SdlEventType.MouseButtonUp:
                ref SdlMouseButtonEvent buEvent = ref sdlEvent.Button;

                @event = new MouseButtonEvent(WindowEventType.MouseButtonUp, (MouseButton) buEvent.Button);

                break;
            
            case SdlEventType.MouseWheel:
                ref SdlMouseWheelEvent wheelEvent = ref sdlEvent.Wheel;

                float x = wheelEvent.PreciseX;
                float y = wheelEvent.PreciseY;
                
                if (wheelEvent.Direction != 0)
                {
                    x = wheelEvent.PreciseX * -1;
                    y = wheelEvent.PreciseY * -1;
                }

                @event = new MouseScrollEvent(x, y);

                break;

            default:
                // Again, filter out unrecognized events.
                // This literally ignores that they ever exist so that PollEvent *always* returns an event that Pie
                // can understand.
                return PollEvent(out @event);
        }

        return true;
    }

    public IWindowEvent[] PollEvents()
    {
        List<IWindowEvent> events = new List<IWindowEvent>();
        while (PollEvent(out IWindowEvent evnt))
            events.Add(evnt);

        return events.ToArray();
    }

    public void Dispose()
    {
        if (_glContext != null)
            Sdl.GLDeleteContext(_glContext);
        
        Sdl.DestroyWindow(_window);
        Sdl.Quit();
    }
}