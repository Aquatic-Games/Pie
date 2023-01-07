using System;

namespace Pie;

public enum GraphicsApi
{
    /// <summary>
    /// OpenGL 3.3, GLSL version 330.
    /// </summary>
    OpenGl33,
    
    /// <summary>
    /// OpenGL ES 2.0, GLSL ES version 100.
    /// </summary>
    OpenGLES20,
    
    /// <summary>
    /// Direct3D 11, shader model 5.0
    /// </summary>
    D3D11,
    
    /// <summary>
    /// !! EXPERIMENTAL - WILL BE SLOW AND BUGGY !! Vulkan 1.0
    /// </summary>
    Vulkan
}

public static class GraphicsApiExtensions
{
    /// <summary>
    /// Get the "friendly" name of the API.
    /// </summary>
    /// <param name="api">The API.</param>
    /// <returns>The "friendly" name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">An invalid API was provided.</exception>
    public static string ToFriendlyString(this GraphicsApi api)
    {
        return api switch
        {
            GraphicsApi.OpenGl33 => "OpenGL 3.3",
            GraphicsApi.D3D11 => "DirectX 11",
            _ => throw new ArgumentOutOfRangeException(nameof(api), api, null)
        };
    }
}