using System.Runtime.InteropServices;
using System.Text;

namespace Pie.SDL;

public static unsafe class Sdl
{
    public const string SdlName = "SDL2";

    public const uint InitAudio = 0x10;
    public const uint InitVideo = 0x20;
    public const uint InitEvents = 0x4000;

    public const uint WindowposCentered = 0x2FFF0000;

    public const int Query = -1;
    public const int Disable = 0;
    public const int Enable = 1;

    [DllImport(SdlName, EntryPoint = "SDL_Init")]
    public static extern int Init(uint flags);
    
    [DllImport(SdlName, EntryPoint = "SDL_Quit")]
    public static extern void Quit();

    [DllImport(SdlName, EntryPoint = "SDL_GetError")]
    public static extern sbyte* GetError();

    [DllImport(SdlName, EntryPoint = "SDL_SetHint")]
    public static extern bool SetHint(sbyte* name, sbyte* value);

    [DllImport(SdlName, EntryPoint = "SDL_CreateWindow")]
    public static extern void* CreateWindow(sbyte* title, int x, int y, int w, int h, uint flags);

    [DllImport(SdlName, EntryPoint = "SDL_DestroyWindow")]
    public static extern void DestroyWindow(void* window);

    [DllImport(SdlName, EntryPoint = "SDL_GetWindowSize")]
    public static extern void GetWindowSize(void* window, int* w, int* h);

    [DllImport(SdlName, EntryPoint = "SDL_GetWindowSizeInPixels")]
    public static extern void GetWindowSizeInPixels(void* window, int* w, int* h);

    [DllImport(SdlName, EntryPoint = "SDL_GetWindowTitle")]
    public static extern sbyte* GetWindowTitle(void* window);

    [DllImport(SdlName, EntryPoint = "SDL_GetWindowPosition")]
    public static extern void GetWindowPosition(void* window, int* x, int* y);

    [DllImport(SdlName, EntryPoint = "SDL_GetWindowFlags")]
    public static extern SdlWindowFlags GetWindowFlags(void* window);
    
    [DllImport(SdlName, EntryPoint = "SDL_GetWindowGrab")]
    public static extern bool GetWindowGrab(void* window);
    
    [DllImport(SdlName, EntryPoint = "SDL_GetRelativeMouseMode")]
    public static extern bool GetRelativeMouseMode();
    
    [DllImport(SdlName, EntryPoint = "SDL_GetDisplayMode")]
    public static extern int GetDisplayMode(int displayIndex, int modeIndex, SdlDisplayMode* mode);
    
    [DllImport(SdlName, EntryPoint = "SDL_GetDisplayBounds")]
    public static extern int GetDisplayBounds(int displayIndex, SdlRect* rect);

    [DllImport(SdlName, EntryPoint = "SDL_GetNumDisplayModes")]
    public static extern int GetNumDisplayModes(int displayIndex);
    
    [DllImport(SdlName, EntryPoint = "SDL_GetNumVideoDisplays")]
    public static extern int GetNumVideoDisplays();
    
    [DllImport(SdlName, EntryPoint = "SDL_GetDesktopDisplayMode")]
    public static extern int GetDesktopDisplayMode(int displayIndex, SdlDisplayMode* mode);

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowSize")]
    public static extern void SetWindowSize(void* window, int w, int h);

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowPosition")]
    public static extern void SetWindowPosition(void* window, int x, int y);

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowIcon")]
    public static extern void SetWindowIcon(void* window, void* surface);

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowTitle")]
    public static extern void SetWindowTitle(void* window, sbyte* title);

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowFullscreen")]
    public static extern void SetWindowFullscreen(void* window, SdlWindowFlags flags);
    
    [DllImport(SdlName, EntryPoint = "SDL_SetWindowResizable")]
    public static extern void SetWindowResizable(void* window, bool resizable);
    
    [DllImport(SdlName, EntryPoint = "SDL_SetWindowBordered")]
    public static extern void SetWindowBordered(void* window, bool bordered);

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowGrab")]
    public static extern void SetWindowGrab(void* window, bool grabbed);
    
    [DllImport(SdlName, EntryPoint = "SDL_SetRelativeMouseMode")]
    public static extern void SetRelativeMouseMode(bool enabled);

    [DllImport(SdlName, EntryPoint = "SDL_GL_SetAttribute")]
    public static extern int GLSetAttribute(SdlGlAttr attr, int value);

    [DllImport(SdlName, EntryPoint = "SDL_GL_CreateContext")]
    public static extern void* GLCreateContext(void* window);

    [DllImport(SdlName, EntryPoint = "SDL_GL_DeleteContext")]
    public static extern void GLDeleteContext(void* context);

    [DllImport(SdlName, EntryPoint = "SDL_GL_MakeCurrent")]
    public static extern int GLMakeCurrent(void* window, void* context);

    [DllImport(SdlName, EntryPoint = "SDL_GL_SetSwapInterval")]
    public static extern int GLSetSwapInterval(int interval);

    [DllImport(SdlName, EntryPoint = "SDL_GL_SwapWindow")]
    public static extern void GLSwapWindow(void* window);

    [DllImport(SdlName, EntryPoint = "SDL_GL_GetProcAddress")]
    public static extern void* GLGetProcAddress(sbyte* proc);
    
    [DllImport(SdlName, EntryPoint = "SDL_PollEvent")]
    public static extern bool PollEvent(SdlEvent* @event);

    [DllImport(SdlName, EntryPoint = "SDL_CreateRGBSurfaceWithFormatFrom")]
    public static extern void* CreateRGBSurfaceWithFormatFrom(void* pixels, int width, int height, int depth, int pitch,
        uint format); // returning void* cause I don't use anything stored inside SDL_Surface so why bother writing it

    [DllImport(SdlName, EntryPoint = "SDL_GetWindowWMInfo")]
    public static extern bool GetWindowWMInfo(void* window, SdlSysWmInfo* wmInfo);
    
    [DllImport(SdlName, EntryPoint = "SDL_ShowCursor")]
    public static extern int ShowCursor(int toggle);
    
    #region Safe helpers

    public static string GetErrorS() => Marshal.PtrToStringAnsi((IntPtr) GetError());

    public static bool SetHint(string name, string value)
    {
        fixed (byte* nPtr = Encoding.UTF8.GetBytes(name))
        fixed (byte* vPtr = Encoding.UTF8.GetBytes(value))
            return SetHint((sbyte*) nPtr, (sbyte*) vPtr);
    }

    public static nint GLGetProcAddress(string proc)
    {
        fixed (byte* procPtr = Encoding.UTF8.GetBytes(proc))
            return (nint) GLGetProcAddress((sbyte*) procPtr);
    }

    #endregion
}