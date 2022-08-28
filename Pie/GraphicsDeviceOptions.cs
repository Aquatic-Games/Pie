using System;

namespace Pie;

public struct GraphicsDeviceOptions
{
    public bool Debug;

    public GraphicsDeviceOptions()
    {
        Debug = false;
    }

    public GraphicsDeviceOptions(bool debug)
    {
        Debug = debug;
    }
}