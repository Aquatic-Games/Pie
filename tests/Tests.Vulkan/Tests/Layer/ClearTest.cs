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
    private ImageView[] _swapchainImageViews;
    private CommandBuffer _commandBuffer;
    
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
        _commandBuffer = _vkLayer.CreateCommandBuffer(_device, CommandBufferLevel.Primary);

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

        _swapchainImageViews = new ImageView[_swapchain.Images.Length];
        for (int i = 0; i < _swapchainImageViews.Length; i++)
        {
            ImageViewCreateInfo imageViewInfo = new ImageViewCreateInfo()
            {
                SType = StructureType.ImageViewCreateInfo,

                Image = _swapchain.Images[i],

                ViewType = ImageViewType.Type2D,
                Format = surfaceFormat.Format,

                Components = new ComponentMapping()
                {
                    R = ComponentSwizzle.Identity,
                    G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity,
                    A = ComponentSwizzle.Identity,
                },

                SubresourceRange = new ImageSubresourceRange()
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };

            VkLayer.CheckResult(_vkLayer.Vk.CreateImageView(_device.Device, &imageViewInfo, null,
                out _swapchainImageViews[i]));
        }
    }

    public override unsafe void Dispose()
    {
        foreach (ImageView view in _swapchainImageViews)
            _vkLayer.Vk.DestroyImageView(_device.Device, view, null);
        
        _vkLayer.DestroySwapchain(_device, _swapchain);
        _vkLayer.DestroyDevice(_device);
        _vkLayer.Dispose();
        
        base.Dispose();
    }

    public ClearTest() : base(new Size(1280, 720), "VkLayer clearing test") { }
}