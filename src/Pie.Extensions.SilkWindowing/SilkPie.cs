using System;
using System.Drawing;
using Pie.OpenGL;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

namespace Pie.Extensions.SilkWindowing;

// A beautiful silk pie. It's like a cottage pie, but silky smooth.
public static class SilkPie
{
    public static IWindow CreateWindow(ref WindowOptions options, GraphicsApi api)
    {
        options.ShouldSwapAutomatically = false;
        options.API = api switch
        {
            GraphicsApi.OpenGL => new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 3)),
            GraphicsApi.OpenGLES => new GraphicsAPI(ContextAPI.OpenGLES, ContextProfile.Core, ContextFlags.Default, new APIVersion(3, 0)),
            GraphicsApi.Vulkan => new GraphicsAPI(ContextAPI.Vulkan, new APIVersion(1, 3)),
            GraphicsApi.D3D11 => GraphicsAPI.None,
            GraphicsApi.Null => GraphicsAPI.None,
            _ => throw new ArgumentOutOfRangeException(nameof(api), api, null)
        };

        return Window.Create(options);
    }

    public static IWindow CreateWindow(ref WindowOptions options)
    {
        return CreateWindow(ref options, GraphicsDevice.GetBestApiForPlatform());
    }
    
    /// <summary>
    /// Create a graphics device for an already existing window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="api">The graphics API to use.</param>
    /// <param name="flags">The creation flags for this graphics device, if any.</param>
    /// <returns>The created graphics device.</returns>
    public static GraphicsDevice CreateGraphicsDevice(this IWindow window, GraphicsApi api, GraphicsDeviceOptions options = default)
    {
        Size winSize = new Size(window.Size.X, window.Size.Y);
        switch (api)
        {
            case GraphicsApi.OpenGL:
            case GraphicsApi.OpenGLES:
                IGLContext glContext = window.GLContext;
                PieGlContext context = new PieGlContext(s => glContext.GetProcAddress(s), i =>
                {
                    glContext.SwapInterval(i);
                    glContext.SwapBuffers();
                });
                return GraphicsDevice.CreateOpenGL(context, winSize, api == GraphicsApi.OpenGLES, options);
            case GraphicsApi.D3D11:
                return GraphicsDevice.CreateD3D11(window.Native!.Win32!.Value.Hwnd, winSize, options);
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
    }

    /// <summary>
    /// Create a graphics device for an already existing window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <returns>The created graphics device.</returns>
    public static GraphicsDevice CreateGraphicsDevice(this IWindow window)
    {
        return CreateGraphicsDevice(window, GraphicsDevice.GetBestApiForPlatform());
    }
}