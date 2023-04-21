using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pie.Windowing.SdlNative;

public static unsafe class Sdl
{
    public const string SdlName = "SDL2";

    public const uint InitAudio = 0x10;
    public const uint InitVideo = 0x20;
    public const uint InitEvents = 0x4000;

    public const uint WindowposCentered = 0x2FFF0000;

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

    [DllImport(SdlName, EntryPoint = "SDL_SetWindowSize")]
    public static extern void SetWindowSize(void* window, int w, int h);

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