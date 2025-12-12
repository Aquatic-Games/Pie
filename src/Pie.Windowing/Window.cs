using System;
using System.Collections.Generic;
using System.Drawing;
using Pie.OpenGL;
using Pie.Windowing.Events;
using Silk.NET.SDL;
using MouseButtonEvent = Pie.Windowing.Events.MouseButtonEvent;
using Point = System.Drawing.Point;
using QuitEvent = Pie.Windowing.Events.QuitEvent;
using TextInputEvent = Pie.Windowing.Events.TextInputEvent;
using SdlWindow = Silk.NET.SDL.Window;
using static Pie.Windowing.SdlHelper;

namespace Pie.Windowing;

/// <summary>
/// Represents a window that can be rendered to.
/// </summary>
public sealed unsafe class Window : IDisposable
{
    private SdlWindow* _window;
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
            SDL.GetWindowSize(_window, &width, &height);
            return new Size(width, height);
        }

        set => SDL.SetWindowSize(_window, value.Width, value.Height);
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
            SDL.GetWindowSizeInPixels(_window, &width, &height);
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
            SDL.GetWindowPosition(_window, &x, &y);
            return new Point(x, y);
        }
        set => SDL.SetWindowPosition(_window, value.X, value.Y);
    }

    /// <summary>
    /// Get or set the title of the window.
    /// </summary>
    public string Title
    {
        get => SDL.GetWindowTitleS(_window);
        set => SDL.SetWindowTitle(_window, value);
    }

    /// <summary>
    /// Get or set the window <see cref="Pie.Windowing.FullscreenMode"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public FullscreenMode FullscreenMode
    {
        get
        {
            WindowFlags flags = (WindowFlags) SDL.GetWindowFlags(_window);

            if ((flags & WindowFlags.FullscreenDesktop) == WindowFlags.FullscreenDesktop)
                return FullscreenMode.BorderlessFullscreen;
            if ((flags & WindowFlags.Fullscreen) == WindowFlags.Fullscreen)
                return FullscreenMode.ExclusiveFullscreen;

            return FullscreenMode.Windowed;
        }
        set
        {
            WindowFlags flags = value switch
            {
                FullscreenMode.Windowed => 0,
                FullscreenMode.ExclusiveFullscreen => WindowFlags.Fullscreen,
                FullscreenMode.BorderlessFullscreen => WindowFlags.FullscreenDesktop,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            SDL.SetWindowFullscreen(_window, (uint) flags);
        }
    }

    /// <summary>
    /// Get or set the window <see cref="Pie.Windowing.CursorMode"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CursorMode CursorMode
    {
        get
        {
            bool visible = SDL.ShowCursor(-1) == Sdl.Enable;
            bool grabbed = SDL.GetWindowGrab(_window) == SdlBool.True;
            bool relative = SDL.GetRelativeMouseMode() == SdlBool.True;

            if (!grabbed && !relative)
                return visible ? CursorMode.Visible : CursorMode.Hidden;

            return relative ? CursorMode.Locked : CursorMode.Grabbed;
        }
        set
        {
            switch (value)
            {
                case CursorMode.Visible:
                    SDL.SetRelativeMouseMode(SdlBool.False);
                    SDL.SetWindowGrab(_window, SdlBool.False);
                    SDL.ShowCursor(Sdl.Enable);
                    break;
                case CursorMode.Hidden:
                    SDL.SetRelativeMouseMode(SdlBool.False);
                    SDL.SetWindowGrab(_window, SdlBool.False);
                    SDL.ShowCursor(Sdl.Disable);
                    break;
                case CursorMode.Grabbed:
                    SDL.SetRelativeMouseMode(SdlBool.False);
                    SDL.SetWindowGrab(_window, SdlBool.True);
                    SDL.ShowCursor(Sdl.Enable);
                    break;
                case CursorMode.Locked:
                    SDL.SetRelativeMouseMode(SdlBool.True);
                    SDL.SetWindowGrab(_window, SdlBool.True);
                    SDL.ShowCursor(Sdl.Disable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    /// <summary>
    /// If true, the window should able to be resized.
    /// </summary>
    public bool Resizable
    {
        get => ((WindowFlags) SDL.GetWindowFlags(_window) & WindowFlags.Resizable) == WindowFlags.Resizable;
        set => SDL.SetWindowResizable(_window, value ? SdlBool.True : SdlBool.False);
    }

    /// <summary>
    /// If true, the window should not have a border.
    /// </summary>
    /// <remarks>This is <b>not</b> the same as <see cref="Pie.Windowing.FullscreenMode.BorderlessFullscreen"/>.</remarks>
    public bool Borderless
    {
        get => ((WindowFlags) SDL.GetWindowFlags(_window) & WindowFlags.Borderless) == WindowFlags.Borderless;
        set => SDL.SetWindowBordered(_window, !value ? SdlBool.True : SdlBool.False);
    }

    /// <summary>
    /// Get/set the window visibility. Making the window invisible should also remove it from the taskbar.
    /// </summary>
    public bool Visible
    {
        get => ((WindowFlags) SDL.GetWindowFlags(_window) & WindowFlags.Shown) == WindowFlags.Shown;
        set
        {
            if (value)
                SDL.ShowWindow(_window);
            else
                SDL.HideWindow(_window);
        }
    }

    /// <summary>
    /// If true, the window should be the window manager's currently focused window, and the window is ready to accept
    /// input from the user.
    /// </summary>
    public bool Focused => ((WindowFlags) SDL.GetWindowFlags(_window) & WindowFlags.InputFocus) == WindowFlags.InputFocus;

    internal Window(WindowBuilder builder)
    {
        if (SDL.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new PieException($"SDL failed to initialize: {SDL.GetErrorS()}");
        
        // TODO: Disable/make optional.
        // I simply disable this cause I find it annoying during development.
        // I *would* use wayland but it no worky on my 1060 for whatever reason and I am not bothered enough to fix.
        SDL.SetHint("SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR", "0");

        Point position = builder.WindowPosition ?? new Point(Sdl.WindowposCentered, Sdl.WindowposCentered);

        WindowFlags flags = WindowFlags.None;

        if (builder.WindowResizable)
            flags |= WindowFlags.Resizable;
        if (builder.WindowBorderless)
            flags |= WindowFlags.Borderless;
        if (builder.WindowHidden)
            flags |= WindowFlags.Hidden;
        if (builder.WindowMaximized)
            flags |= WindowFlags.Maximized;
        if (builder.WindowMinimized)
            flags |= WindowFlags.Minimized;

        flags |= builder.WindowFullscreenMode switch
        {
            FullscreenMode.Windowed => WindowFlags.None,
            FullscreenMode.ExclusiveFullscreen => WindowFlags.Fullscreen,
            FullscreenMode.BorderlessFullscreen => WindowFlags.FullscreenDesktop,
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (builder.WindowApi)
        {
            case GraphicsApi.OpenGL:
            case GraphicsApi.OpenGLES:
                flags |= WindowFlags.Opengl;
                SDL.GLSetAttribute(GLattr.ContextMajorVersion, 4);
                SDL.GLSetAttribute(GLattr.ContextMinorVersion, 3);
                SDL.GLSetAttribute(GLattr.ContextProfileMask,
                    builder.WindowApi == GraphicsApi.OpenGLES ? (int) GLprofile.ES : (int) GLprofile.Core);

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

                SDL.GLSetAttribute(GLattr.RedSize, bits.r);
                SDL.GLSetAttribute(GLattr.GreenSize, bits.g);
                SDL.GLSetAttribute(GLattr.BlueSize, bits.b);
                SDL.GLSetAttribute(GLattr.AlphaSize, bits.a);

                SDL.GLSetAttribute(GLattr.DepthSize, depthBits.depth);
                SDL.GLSetAttribute(GLattr.StencilSize, depthBits.stencil);
                
                SDL.GLSetAttribute(GLattr.FramebufferSrgbCapable, bits.srgb ? 1 : 0);
                SDL.GLSetAttribute(GLattr.Floatbuffers, bits.fp ? 1 : 0);
                
                break;
            case GraphicsApi.D3D11:
            case GraphicsApi.Null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _window = SDL.CreateWindow(builder.WindowTitle, position.X, position.Y, builder.WindowSize.Width,
            builder.WindowSize.Height, (uint) flags);

        if (_window == null)
        {
            SDL.Quit();
            throw new PieException($"Window failed to create. {SDL.GetErrorS()}");
        }

        if (builder.WindowIcon != null)
        {
            Icon icon = builder.WindowIcon.Value;
            Surface* surface;
            fixed (void* ptr = icon.Data)
            {
                // ABGR ?????
                // The hell endianness has SDL been compiled in?
                surface = SDL.CreateRGBSurfaceWithFormatFrom(ptr, (int) icon.Width, (int) icon.Height, 0,
                    (int) icon.Width * 4, (uint) PixelFormatEnum.Abgr8888);
            }

            SDL.SetWindowIcon(_window, surface);
        }

        if (builder.WindowApi is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
        {
            _glContext = SDL.GLCreateContext(_window);
            if (_glContext == null)
                throw new PieException($"Failed to create GL context. {SDL.GetErrorS()}");

            // Juuust make sure the context is current, even though it should already be.
            if (SDL.GLMakeCurrent(_window, _glContext) < 0)
                throw new PieException($"Failed to make GL context current. {SDL.GetErrorS()}");
        }

        _api = builder.WindowApi;
    }

    /// <summary>
    /// Focus the window if it is not focused, bringing it to the front if necessary.
    /// </summary>
    public void Focus() => SDL.RaiseWindow(_window);

    /// <summary>
    /// Centers the window on the primary monitor.
    /// </summary>
    public void Center() => SDL.SetWindowPosition(_window, Sdl.WindowposCentered, Sdl.WindowposCentered);

    /// <summary>
    /// Maximises the window, restoring it if necessary.
    /// </summary>
    public void Maximize() => SDL.MaximizeWindow(_window);

    /// <summary>
    /// Minimises the window.
    /// </summary>
    public void Minimize() => SDL.MinimizeWindow(_window);

    /// <summary>
    /// Restores the window to its initial state, before it was minimised.
    /// </summary>
    public void Restore() => SDL.RestoreWindow(_window);

    /// <summary>
    /// Creates a <see cref="GraphicsDevice"/> from this window.
    /// </summary>
    /// <param name="options">The <see cref="GraphicsDeviceOptions"/> to use on creation, if any.</param>
    /// <returns>The created <see cref="GraphicsDevice"/>.</returns>
    public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions? options = null)
    {
        int width, height;
        
        SDL.GetWindowSizeInPixels(_window, &width, &height);
        Size size = new Size(width, height);
        
        switch (_api)
        {
            case GraphicsApi.OpenGL:
            case GraphicsApi.OpenGLES:
                return GraphicsDevice.CreateOpenGL(new PieGlContext(s => (nint) SDL.GLGetProcAddress(s), i =>
                {
                    SDL.GLSetSwapInterval(i);
                    SDL.GLSwapWindow(_window);
                }), size, _api == GraphicsApi.OpenGLES, options ?? new GraphicsDeviceOptions());
            
            case GraphicsApi.D3D11:
                SysWMInfo info = new SysWMInfo();
                SDL.GetWindowWMInfo(_window, &info);
                return GraphicsDevice.CreateD3D11(info.Info.Win.Hwnd, size, options ?? new GraphicsDeviceOptions());
            
            case GraphicsApi.Null:
                return GraphicsDevice.CreateNull(size, options ?? new GraphicsDeviceOptions());
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Poll the next window event, if there are any remaining.
    /// </summary>
    /// <param name="event">The polled window event.</param>
    /// <returns>True if an event was processed, false otherwise.</returns>
    public bool PollEvent(out IWindowEvent @event)
    {
        Event sdlEvent;
        @event = null;
        if (SDL.PollEvent(&sdlEvent) == 0)
            return false;

        if (!HandleSdlEvent(ref sdlEvent, out @event))
            return PollEvent(out @event);

        return true;
    }

    public bool PollEvent()
    {
        return SDL.PollEvent(null) != 0;
    }
    
    /// <summary>
    /// Polls events and returns it as an IEnumerable. This method simply calls <see cref="PollEvent"/> under the hood,
    /// but is a more "C# friendly" way of doing things.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of events.</returns>
    public IEnumerable<IWindowEvent> PollEvents()
    {
        while (PollEvent(out IWindowEvent winEvent))
            yield return winEvent;
    }

    public bool WaitEvent(out IWindowEvent @event)
    {
        Event sdlEvent;
        @event = null;
        if (SDL.WaitEvent(&sdlEvent) == 0)
            return false;

        if (!HandleSdlEvent(ref sdlEvent, out @event))
            return WaitEvent(out @event);
        
        return true;
    }

    public bool WaitEvent(out IWindowEvent @event, int timeout)
    {
        Event sdlEvent;
        @event = null;
        if (SDL.WaitEventTimeout(&sdlEvent, timeout) == 0)
            return false;

        if (!HandleSdlEvent(ref sdlEvent, out @event))
            return WaitEvent(out @event, timeout);

        return true;
    }

    public bool WaitEvent()
    {
        return SDL.WaitEvent(null) != 0;
    }

    public bool WaitEvent(int timeout)
    {
        return SDL.WaitEventTimeout(null, timeout) != 0;
    }

    /// <summary>
    /// Dispose of this window.
    /// </summary>
    public void Dispose()
    {
        if (_glContext != null)
            SDL.GLDeleteContext(_glContext);
        
        SDL.DestroyWindow(_window);
        SDL.Quit();
    }

    private bool HandleSdlEvent(ref Event sdlEvent, out IWindowEvent @event)
    {
        switch ((EventType) sdlEvent.Type)
        {
            case EventType.Quit:
                @event = new QuitEvent();
                break;
            
            case EventType.Windowevent:
                switch ((WindowEventID) sdlEvent.Window.Event)
                {
                    case WindowEventID.Resized:
                        @event = new ResizeEvent(sdlEvent.Window.Data1, sdlEvent.Window.Data2);
                        break;
                    default:
                        // Filter out unrecognized events.
                        @event = null;
                        return false;
                }

                break;
            
            case EventType.Keydown:
                ref KeyboardEvent kde = ref sdlEvent.Key;
                WindowEventType kdeType = kde.Repeat != 0 ? WindowEventType.KeyRepeat : WindowEventType.KeyDown;
                @event = new KeyEvent(kdeType, (uint) kde.Keysym.Scancode, SdlHelper.KeycodeToKey((uint) kde.Keysym.Sym));
                
                break;
            case EventType.Keyup:
                ref KeyboardEvent kue = ref sdlEvent.Key;
                @event = new KeyEvent(WindowEventType.KeyUp, (uint) kue.Keysym.Scancode, SdlHelper.KeycodeToKey((uint) kue.Keysym.Sym));
                
                break;
            
            case EventType.Textinput:
                ref Silk.NET.SDL.TextInputEvent textEvent = ref sdlEvent.Text;
                fixed (byte* text = textEvent.Text)
                    @event = new TextInputEvent(new string((sbyte*) text));

                break;
            
            case EventType.Mousemotion:
                ref MouseMotionEvent motionEvent = ref sdlEvent.Motion;
                @event = new MouseMoveEvent(motionEvent.X, motionEvent.Y, motionEvent.Xrel, motionEvent.Yrel);

                break;
            
            case EventType.Mousebuttondown:
                ref Silk.NET.SDL.MouseButtonEvent bdEvent = ref sdlEvent.Button;
                @event = new MouseButtonEvent(WindowEventType.MouseButtonDown, (MouseButton) bdEvent.Button);

                break;
            
            case EventType.Mousebuttonup:
                ref Silk.NET.SDL.MouseButtonEvent buEvent = ref sdlEvent.Button;
                @event = new MouseButtonEvent(WindowEventType.MouseButtonUp, (MouseButton) buEvent.Button);

                break;
            
            case EventType.Mousewheel:
                ref MouseWheelEvent wheelEvent = ref sdlEvent.Wheel;

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
                @event = null;
                return false;
        }

        return true;
    }
}