using System;
using System.Drawing;
using Silk.NET.Windowing;

namespace Pie.Extensions.SilkWindowing;

public static class PieSilk
{
    public static IWindow CreateWindow(WindowOptions options, GraphicsApi api)
    {
        options.ShouldSwapAutomatically = false;
        switch (api)
        {
            case GraphicsApi.OpenGl33:
                options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible,
                    new APIVersion(3, 3));
                break;
            case GraphicsApi.D3D11:
                options.API = GraphicsAPI.None;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
        
        return Window.Create(options);
    }

    public static IWindow CreateWindow(WindowOptions options)
    {
        return CreateWindow(options, GraphicsDevice.GetBestApiForPlatform());
    }
    
    /// <summary>
    /// Create a graphics device for an already existing window.
    /// </summary>
    /// <param name="window">The window object.</param>
    /// <param name="api">The graphics API to use.</param>
    /// <returns>The created graphics device.</returns>
    public static GraphicsDevice CreateGraphicsDevice(this IWindow window, GraphicsApi api)
    {
        Size winSize = new Size(window.Size.X, window.Size.Y);
        switch (api)
        {
            case GraphicsApi.OpenGl33:
                return GraphicsDevice.CreateOpenGL33(window.GLContext, winSize, false);
            case GraphicsApi.D3D11:
                return GraphicsDevice.CreateD3D11(window.Native!.Win32!.Value.Hwnd, winSize, false);
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