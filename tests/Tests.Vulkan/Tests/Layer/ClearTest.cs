using System.Drawing;
using System.Runtime.InteropServices;
using Pie.SDL;
using Pie.Vulkan;

namespace Tests.Vulkan.Tests.Layer;

public class ClearTest : TestBase
{
    private VkLayer _vkLayer;
    
    protected override unsafe void Initialize()
    {
        base.Initialize();

        PieVkContext context = new PieVkContext(() =>
        {
            uint numInstanceExtensions;
            Sdl.VulkanGetInstanceExtensions(Window, &numInstanceExtensions, null);

            sbyte** instancePtrs = (sbyte**) NativeMemory.Alloc((nuint) (numInstanceExtensions * sizeof(sbyte*)));
            Sdl.VulkanGetInstanceExtensions(Window, &numInstanceExtensions, instancePtrs);
            
            string[] instanceExtensions = NativeHelper.PtrToStringArray((nint) instancePtrs, numInstanceExtensions);
            
            NativeMemory.Free(instancePtrs);

            return instanceExtensions;
        }, instance =>
        {
            void** surface;
            Sdl.VulkanCreateSurface(Window, instance, &surface);

            return (nint) surface;
        });

        _vkLayer = new VkLayer(context, true);

        VkLayer.VkPhysicalDevice pDevice = _vkLayer.GetBestPhysicalDevice();
    }

    public override void Dispose()
    {
        _vkLayer.Dispose();
        
        base.Dispose();
    }

    public ClearTest() : base(new Size(1280, 720), "VkLayer clearing test") { }
}