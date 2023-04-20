using System;
using System.Collections.Generic;
using System.Drawing;
using Pie.OpenGL;
using Pie.Windowing.Events;
using Silk.NET.SDL;
using SdlWindow = Silk.NET.SDL.Window;

namespace Pie.Windowing;

public sealed unsafe class Window : IDisposable
{
    private Sdl _sdl;

    private SdlWindow* _window;
    
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
            _sdl.GetWindowSize(_window, &width, &height);
            return new Size(width, height);
        }

        set => _sdl.SetWindowSize(_window, value.Width, value.Height);
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
            _sdl.GetWindowSizeInPixels(_window, &width, &height);
            return new Size(width, height);
        }
    }

    internal Window(WindowBuilder builder)
    {
        _sdl = Sdl.GetApi();

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new PieException($"SDL failed to initialize: {_sdl.GetErrorS()}");
        
        // TODO: Disable/make optional.
        // I simply disable this cause I find it annoying during development.
        // I *would* use wayland but it no worky on my 1060 for whatever reason and I am not bothered enough to fix.
        _sdl.SetHint(Sdl.HintVideoX11NetWMBypassCompositor, "0");

        System.Drawing.Point position = builder.WindowPosition ??
                                        new System.Drawing.Point(Sdl.WindowposCentered, Sdl.WindowposCentered);

        WindowFlags flags = builder.WindowApi switch
        {
            GraphicsApi.OpenGL => WindowFlags.Opengl,
            GraphicsApi.D3D11 => WindowFlags.None,
            GraphicsApi.Vulkan => WindowFlags.Vulkan,
            GraphicsApi.Null => WindowFlags.None,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (builder.WindowResizable)
            flags |= WindowFlags.Resizable;

        if (builder.WindowApi == GraphicsApi.OpenGL)
        {
            _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
            _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
        }

        _window = _sdl.CreateWindow(builder.WindowTitle, position.X, position.Y, builder.WindowSize.Width,
            builder.WindowSize.Height, (uint) flags);

        if (_window == null)
        {
            _sdl.Quit();
            throw new PieException($"Window failed to create. {_sdl.GetErrorS()}");
        }

        if (builder.WindowIcon != null)
        {
            Icon icon = builder.WindowIcon.Value;
            Surface* surface;
            fixed (void* ptr = icon.Data)
            {
                // ABGR ?????
                // The hell endianness has SDL been compiled in?
                surface = _sdl.CreateRGBSurfaceWithFormatFrom(ptr, (int) icon.Width, (int) icon.Height, 0,
                    (int) icon.Width * 4, Sdl.PixelformatAbgr8888);
            }

            _sdl.SetWindowIcon(_window, surface);
            
            Console.WriteLine(_sdl.GetErrorS());
        }

        _glContext = _sdl.GLCreateContext(_window);

        // Juuust make sure the context is current, even though it should already be.
        _sdl.GLMakeCurrent(_window, _glContext);

        _api = builder.WindowApi;
    }

    public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions? options = null)
    {
        int width, height;
        
        _sdl.GetWindowSizeInPixels(_window, &width, &height);
        Size size = new Size(width, height);
        
        switch (_api)
        {
            case GraphicsApi.OpenGL:
                return GraphicsDevice.CreateOpenGL(new PieGlContext(s => (nint) _sdl.GLGetProcAddress(s), i =>
                {
                    _sdl.GLSetSwapInterval(i);
                    _sdl.GLSwapWindow(_window);
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
        Event sdlEvent;
        @event = null;
        if (_sdl.PollEvent(&sdlEvent) != 1)
            return false;

        switch ((EventType) sdlEvent.Type)
        {
            case EventType.Quit:
                @event = new Events.QuitEvent();
                break;
            case EventType.Windowevent:
                switch ((WindowEventID) sdlEvent.Window.Event)
                {
                    case WindowEventID.Resized:
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
            _sdl.GLDeleteContext(_glContext);
        
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}