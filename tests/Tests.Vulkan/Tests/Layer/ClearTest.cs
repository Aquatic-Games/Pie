using System.Drawing;
using System.Runtime.InteropServices;
using Pie.SDL;
using Pie.Vulkan;
using Silk.NET.Vulkan;

namespace Tests.Vulkan.Tests.Layer;

public class ClearTest : TestBase
{
    private VkLayer _vkLayer;
    private VkLayer.VkDevice _device;
    private VkLayer.VkSwapchain _swapchain;
    
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
        _device = _vkLayer.CreateDevice(pDevice);

        SurfaceFormatKHR surfaceFormat = pDevice.SupportedFormats[0];
        foreach (SurfaceFormatKHR format in pDevice.SupportedFormats)
        {
            // try to pick this format if supported.
            if (format.Format == Format.B8G8R8A8Unorm && format.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
            {
                surfaceFormat = format;
                break;
            }
        }

        PresentModeKHR presentMode = PresentModeKHR.FifoKhr;

        Extent2D extent = pDevice.SurfaceCapabilities.CurrentExtent;
        SurfaceTransformFlagsKHR transform = pDevice.SurfaceCapabilities.CurrentTransform;

        uint imageCount = pDevice.SurfaceCapabilities.MinImageCount + 1;

        _swapchain = _vkLayer.CreateSwapchain(_device, surfaceFormat, presentMode, extent, imageCount, transform);
    }

    public override void Dispose()
    {
        _vkLayer.DestroySwapchain(_device, _swapchain);
        _vkLayer.DestroyDevice(_device);
        _vkLayer.Dispose();
        
        base.Dispose();
    }

    public ClearTest() : base(new Size(1280, 720), "VkLayer clearing test") { }
}