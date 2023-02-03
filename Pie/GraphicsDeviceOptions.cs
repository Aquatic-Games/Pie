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
    /// Create a new <see cref="GraphicsDeviceOptions"/> with the default settings.
    /// </summary>
    public GraphicsDeviceOptions()
    {
        Debug = false;
    }

    /// <summary>
    /// Create a new <see cref="GraphicsDeviceOptions"/>.
    /// </summary>
    /// <param name="debug">If enabled, the graphics device will run in debug mode.</param>
    public GraphicsDeviceOptions(bool debug)
    {
        Debug = debug;
    }
}