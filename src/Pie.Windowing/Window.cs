using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Pie.OpenGL;
using Pie.Windowing.Events;
using Pie.Windowing.SdlNative;

namespace Pie.Windowing;

public sealed unsafe class Window : IDisposable
{
    private void* _window;
    
    private void* _glContext;

    private GraphicsApi _api;

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

    internal Window(WindowBuilder builder)
    {
        if (Sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new PieException($"SDL failed to initialize: {Sdl.GetErrorS()}");
        
        // TODO: Disable/make optional.
        // I simply disable this cause I find it annoying during development.
        // I *would* use wayland but it no worky on my 1060 for whatever reason and I am not bothered enough to fix.
        Sdl.SetHint("SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR", "0");

        Point position = builder.WindowPosition ?? new Point((int) Sdl.WindowposCentered, (int) Sdl.WindowposCentered);

        SdlWindowFlags flags = builder.WindowApi switch
        {
            GraphicsApi.OpenGL => SdlWindowFlags.OpenGL,
            GraphicsApi.D3D11 => SdlWindowFlags.None,
            GraphicsApi.Vulkan => SdlWindowFlags.Vulkan,
            GraphicsApi.Null => SdlWindowFlags.None,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (builder.WindowResizable)
            flags |= SdlWindowFlags.Resizable;

        if (builder.WindowApi == GraphicsApi.OpenGL)
        {
            Sdl.GLSetAttribute(SdlGlAttr.ContextMajorVersion, 4);
            Sdl.GLSetAttribute(SdlGlAttr.ContextMinorVersion, 3);
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

        _glContext = Sdl.GLCreateContext(_window);

        // Juuust make sure the context is current, even though it should already be.
        Sdl.GLMakeCurrent(_window, _glContext);

        _api = builder.WindowApi;
    }

    public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions? options = null)
    {
        int width, height;
        
        Sdl.GetWindowSizeInPixels(_window, &width, &height);
        Size size = new Size(width, height);
        
        switch (_api)
        {
            case GraphicsApi.OpenGL:
                return GraphicsDevice.CreateOpenGL(new PieGlContext(Sdl.GLGetProcAddress, i =>
                {
                    Sdl.GLSetSwapInterval(i);
                    Sdl.GLSwapWindow(_window);
                }), size, options ?? new GraphicsDeviceOptions());
            
            case GraphicsApi.D3D11:
                throw new NotImplementedException();
                break;
            case GraphicsApi.Vulkan:
                throw new NotSupportedException("Vulkan does not actually exist and this API should be removed.");
                break;
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
                        @event = new ResizeEvent(new Size(sdlEvent.Window.Data1, sdlEvent.Window.Data2));
                        break;
                }

                break;
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