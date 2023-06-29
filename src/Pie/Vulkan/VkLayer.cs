using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;

namespace Pie.Vulkan;

// Acts as a minimal abstraction over vulkan.
public unsafe class VkLayer : IDisposable
{
    public Vk Vk;
    public Instance Instance;

    public VkLayer(PieVkContext context, bool debug)
    {
        Vk = Vk.GetApi();
        
        using PinnedString appName = new PinnedString("Pie Application");
        using PinnedString engineName = new PinnedString("Pie");

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) appName.Handle,
            PEngineName = (byte*) engineName.Handle,
            ApiVersion = Vk.Version13
        };

        string[] instanceExtensions = context.GetInstanceExtensions();

        if (debug)
        {
            Array.Resize(ref instanceExtensions, instanceExtensions.Length + 1);
            instanceExtensions[^1] = ExtDebugUtils.ExtensionName;
        }

        using PinnedStringArray instanceExtPtrs = new PinnedStringArray(instanceExtensions);

        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            PpEnabledExtensionNames = (byte**) instanceExtPtrs.Handle,
            EnabledExtensionCount = (uint) instanceExtensions.Length
        };
        
        CheckResult(Vk.CreateInstance(&instanceInfo, null, out Instance));
    }

    public void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
    }

    public static void CheckResult(Result result)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan operation failed. Result: {result}");
    }
}