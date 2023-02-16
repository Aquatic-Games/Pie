using System;

namespace Pie;

/// <summary>
/// The backend graphics API used for a <see cref="GraphicsDevice"/>.
/// </summary>
public enum GraphicsApi
{
    /// <summary>
    /// OpenGL, GLSL version 430.
    /// </summary>
    OpenGL,

    /// <summary>
    /// Direct3D 11, shader model 5.0
    /// </summary>
    D3D11,
    
    /// <summary>
    /// !! EXPERIMENTAL - WILL BE SLOW AND BUGGY !! Vulkan
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
            GraphicsApi.OpenGL => "OpenGL",
            GraphicsApi.D3D11 => "DirectX 11",
            GraphicsApi.Vulkan => "Vulkan",
            _ => throw new ArgumentOutOfRangeException(nameof(api), api, null)
        };
    }
}