using System;
using System.Drawing;
using System.Text;
using Pie.SDL;

namespace Tests.Vulkan;

public abstract unsafe class TestBase : IDisposable
{
    private Size _size;
    private string _title;

    public void* Window;
    
    protected TestBase(Size size, string title)
    {
        _size = size;
        _title = title;
    }

    protected virtual void Initialize() { }

    protected virtual void Update() { }

    protected virtual void Draw() { }

    public void Run()
    {
        Sdl.SetHint("SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR", "0");
        
        if (Sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception("SDL failed to initialize.");

        fixed (byte* title = Encoding.ASCII.GetBytes(_title))
        {
            Window = Sdl.CreateWindow((sbyte*) title, (int) Sdl.WindowposCentered, (int) Sdl.WindowposCentered,
                _size.Width, _size.Height, (uint) SdlWindowFlags.Vulkan);
        }
        
        Initialize();

        bool shouldClose = false;
        while (!shouldClose)
        {
            SdlEvent sdlEvent;
            while (Sdl.PollEvent(&sdlEvent))
            {
                if (sdlEvent.Type == (uint) SdlEventType.Quit)
                    shouldClose = true;
            }
            
            Update();
            Draw();
        }
    }

    public virtual void Dispose()
    {
        Sdl.DestroyWindow(Window);
        Sdl.Quit();
    }
}