namespace Pie.Vulkan;

public unsafe interface IVkContext
{
    public byte** GetInstanceExtensions(out nuint numExtensions);
}