using System;

namespace Pie.Vulkan;

public class PieVkContext
{
    public Func<string[]> GetInstanceExtensions;
    
    public Func<nint, nint> CreateSurface;

    public PieVkContext(Func<string[]> getInstanceExtensions, Func<nint, nint> createSurface)
    {
        GetInstanceExtensions = getInstanceExtensions;
        CreateSurface = createSurface;
    }
}