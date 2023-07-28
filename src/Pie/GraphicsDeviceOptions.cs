using System;

namespace Pie;

/// <summary>
/// Provides various startup options for a <see cref="GraphicsDevice"/>.
/// </summary>
public struct GraphicsDeviceOptions
{
    /// <summary>
    /// If enabled, the graphics device will run in debug mode.
    /// </summary>
    public bool Debug;

    /// <summary>
    /// The format of the main color buffer.
    /// </summary>
    public Format ColorBufferFormat;

    /// <summary>
    /// The format of the depth-stencil buffer. Set to null to disable the depth-stencil buffer.
    /// </summary>
    public Format? DepthStencilBufferFormat;

    /// <summary>
    /// Create a new <see cref="GraphicsDeviceOptions"/> with the default settings.
    /// </summary>
    public GraphicsDeviceOptions()
    {
        Debug = false;
        ColorBufferFormat = Format.B8G8R8A8_UNorm;
        DepthStencilBufferFormat = Format.D24_UNorm_S8_UInt;
    }

    /// <summary>
    /// Create a new <see cref="GraphicsDeviceOptions"/>.
    /// </summary>
    /// <param name="debug">If enabled, the graphics device will run in debug mode.</param>
    public GraphicsDeviceOptions(bool debug = false, Format colorBufferFormat = Format.B8G8R8A8_UNorm,
        Format? depthStencilBufferFormat = Format.D24_UNorm_S8_UInt)
    {
        Debug = debug;
        ColorBufferFormat = colorBufferFormat;
        DepthStencilBufferFormat = depthStencilBufferFormat;
    }
}