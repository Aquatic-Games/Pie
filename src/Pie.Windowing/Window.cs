using System;
using System.Collections.Generic;
using Pie.Windowing.Events;
using Silk.NET.SDL;
using SdlWindow = Silk.NET.SDL.Window;

namespace Pie.Windowing;

public sealed unsafe class Window : IDisposable
{
    private Sdl _sdl;

    private SdlWindow* _window;
    
    private void* _glContext;

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

        SdlWindow* window = _sdl.CreateWindow(builder.WindowTitle, position.X, position.Y, builder.WindowSize.Width,
            builder.WindowSize.Height, (uint) flags);

        if (window == null)
        {
            _sdl.Quit();
            throw new PieException($"Window failed to create. {_sdl.GetErrorS()}");
        }

        _glContext = _sdl.GLCreateContext(window);

        // Juuust make sure the context is current, even though it should already be.
        _sdl.GLMakeCurrent(_window, _glContext);
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