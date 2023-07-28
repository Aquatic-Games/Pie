using System;
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
    private Semaphore _imgAvailable;
    private Semaphore _renderFinished;
    private Fence _inFlightFence;

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

        Console.WriteLine("Creating image views.");
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
        
        Console.WriteLine("Creating synchronization objects.");

        SemaphoreCreateInfo semaphoreCreateInfo = new SemaphoreCreateInfo()
        {
            SType = StructureType.SemaphoreCreateInfo,
        };
        
        VkLayer.CheckResult(_vkLayer.Vk.CreateSemaphore(_device.Device, &semaphoreCreateInfo, null, out _imgAvailable));
        VkLayer.CheckResult(_vkLayer.Vk.CreateSemaphore(_device.Device, &semaphoreCreateInfo, null, out _renderFinished));

        FenceCreateInfo fenceCreateInfo = new FenceCreateInfo()
        {
            SType = StructureType.FenceCreateInfo,
            Flags = FenceCreateFlags.SignaledBit
        };
        
        VkLayer.CheckResult(_vkLayer.Vk.CreateFence(_device.Device, &fenceCreateInfo, null, out _inFlightFence));
    }

    protected override unsafe void Draw()
    {
        base.Draw();

        ref Vk vk = ref _vkLayer.Vk;

        vk.WaitForFences(_device.Device, 1, _inFlightFence, true, ulong.MaxValue);
        vk.ResetFences(_device.Device, 1, _inFlightFence);

        uint imgIndex;
        _vkLayer.SwapchainExt.AcquireNextImage(_device.Device, _swapchain.Swapchain, ulong.MaxValue, _imgAvailable,
            new Fence(), &imgIndex);

        vk.ResetCommandBuffer(_commandBuffer, CommandBufferResetFlags.None);

        RenderingAttachmentInfo colorBufferAttachment = new RenderingAttachmentInfo()
        {
            SType = StructureType.RenderingAttachmentInfo,
            ImageView = _swapchainImageViews[imgIndex],
            ImageLayout = ImageLayout.AttachmentOptimal,
            LoadOp = AttachmentLoadOp.Clear,
            StoreOp = AttachmentStoreOp.Store,
            ClearValue = new ClearValue(new ClearColorValue(1.0f, 0.5f, 0.25f, 1.0f))
        };

        RenderingInfo renderingInfo = new RenderingInfo()
        {
            SType = StructureType.RenderingInfo,
            RenderArea = new Rect2D(new Offset2D(0, 0), new Extent2D(1280, 720)),
            LayerCount = 1,
            ColorAttachmentCount = 1,
            PColorAttachments = &colorBufferAttachment
        };
        
        _vkLayer.CommandBufferBegin(_commandBuffer, renderingInfo, _swapchain.Images[imgIndex]);
        _vkLayer.CommandBufferEnd(_commandBuffer, _swapchain.Images[imgIndex]);
        
        _vkLayer.SwapchainPresent(_device, _swapchain, _commandBuffer, imgIndex, _inFlightFence, _renderFinished, _imgAvailable);
    }

    public override unsafe void Dispose()
    {
        _vkLayer.Vk.DeviceWaitIdle(_device.Device);
        
        _vkLayer.Vk.DestroyFence(_device.Device, _inFlightFence, null);
        _vkLayer.Vk.DestroySemaphore(_device.Device, _imgAvailable, null);
        _vkLayer.Vk.DestroySemaphore(_device.Device, _renderFinished, null);
        
        foreach (ImageView view in _swapchainImageViews)
            _vkLayer.Vk.DestroyImageView(_device.Device, view, null);
        
        _vkLayer.DestroySwapchain(_device, _swapchain);
        _vkLayer.DestroyDevice(_device);
        _vkLayer.Dispose();
        
        base.Dispose();
    }

    public ClearTest() : base(new Size(1280, 720), "VkLayer clearing test") { }
}