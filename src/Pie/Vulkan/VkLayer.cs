using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Pie.Vulkan;

// Acts as a minimal abstraction over vulkan.
public unsafe class VkLayer : IDisposable
{
    public Vk Vk;
    public Instance Instance;

    private string[] _layers;
    private string[] _deviceExtensions;

    private ExtDebugUtils _debugUtils;
    private DebugUtilsMessengerEXT _debugMessenger;

    private KhrSurface _surfaceExt;
    private SurfaceKHR _surface;

    public KhrSwapchain SwapchainExt;

    public VkLayer(PieVkContext context, bool debug)
    {
        PieLog.Log(LogType.Verbose, "Creating VK layer.");

        _deviceExtensions = new[]
        {
            KhrSwapchain.ExtensionName
        };
        
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

            _layers = new[]
            {
                "VK_LAYER_KHRONOS_validation"
            };
        }
        else
            _layers = Array.Empty<string>();
        
        PieLog.Log(LogType.Verbose, "Instance extensions: " + string.Join(", ", instanceExtensions));
        PieLog.Log(LogType.Verbose, "Layers: " + string.Join(", ", _layers));
        PieLog.Log(LogType.Verbose, "Device extensions: " + string.Join(", ", _deviceExtensions));

        using PinnedStringArray instanceExtPtrs = new PinnedStringArray(instanceExtensions);
        using PinnedStringArray layersPtr = new PinnedStringArray(_layers);

        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            PpEnabledExtensionNames = (byte**) instanceExtPtrs.Handle,
            EnabledExtensionCount = instanceExtPtrs.Length,
            
            PpEnabledLayerNames = (byte**) layersPtr.Handle,
            EnabledLayerCount = layersPtr.Length
        };
        
        CheckResult(Vk.CreateInstance(&instanceInfo, null, out Instance));

        if (debug)
        {
            PieLog.Log(LogType.Verbose, "Debug is enabled.");
            
            PieLog.Log(LogType.Verbose, "Getting debug utils.");
            Vk.TryGetInstanceExtension(Instance, out _debugUtils);
            
            PieLog.Log(LogType.Verbose, "Creating debug messenger.");
            DebugUtilsMessengerCreateInfoEXT messengerCreateInfo = new DebugUtilsMessengerCreateInfoEXT()
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugCallback)
            };

            CheckResult(
                _debugUtils.CreateDebugUtilsMessenger(Instance, &messengerCreateInfo, null, out _debugMessenger));
        }
        
        PieLog.Log(LogType.Verbose, "Creating window surface.");
        Vk.TryGetInstanceExtension(Instance, out _surfaceExt);
        _surface = new SurfaceKHR((ulong) context.CreateSurface(Instance.Handle));
    }

    public VkPhysicalDevice GetBestPhysicalDevice()
    {
        uint numPhysicalDevices;
        CheckResult(Vk.EnumeratePhysicalDevices(Instance, &numPhysicalDevices, null));

        PhysicalDevice[] devices = new PhysicalDevice[numPhysicalDevices];
        
        fixed (PhysicalDevice* devicePtr = devices)
            CheckResult(Vk.EnumeratePhysicalDevices(Instance, &numPhysicalDevices, devicePtr));

        PhysicalDevice device = devices[0];
        
        uint numQueueFamilies;
        Vk.GetPhysicalDeviceQueueFamilyProperties(device, &numQueueFamilies, null);

        QueueFamilyProperties[] queueProps = new QueueFamilyProperties[numQueueFamilies];

        fixed (QueueFamilyProperties* queuePropsPtr = queueProps)
            Vk.GetPhysicalDeviceQueueFamilyProperties(device, &numQueueFamilies, queuePropsPtr);

        QueueFamilyIndices indices = new QueueFamilyIndices();

        uint i = 0;
        foreach (QueueFamilyProperties property in queueProps)
        {
            if ((property.QueueFlags & QueueFlags.GraphicsBit) == QueueFlags.GraphicsBit)
                indices.GraphicsQueue = i;

            _surfaceExt.GetPhysicalDeviceSurfaceSupport(device, i, _surface, out Bool32 canPresent);

            if (canPresent)
                indices.PresentQueue = i;

            if (indices.IsComplete)
                break;

            i++;
        }

        if (!indices.IsComplete)
            throw new Exception("Device is not supported.");
        
        _surfaceExt.GetPhysicalDeviceSurfaceCapabilities(device, _surface, out SurfaceCapabilitiesKHR surfaceCapabilities);
        SurfaceFormatKHR[] surfaceFormats;
        PresentModeKHR[] presentModes;

        uint formatCount;
        _surfaceExt.GetPhysicalDeviceSurfaceFormats(device, _surface, &formatCount, null);

        if (formatCount == 0)
            throw new Exception("Device is not supported.");

        surfaceFormats = new SurfaceFormatKHR[formatCount];
        fixed (SurfaceFormatKHR* formatPtr = surfaceFormats)
            _surfaceExt.GetPhysicalDeviceSurfaceFormats(device, _surface, &formatCount, formatPtr);

        uint presentModeCount;
        _surfaceExt.GetPhysicalDeviceSurfacePresentModes(device, _surface, &presentModeCount, null);

        if (presentModeCount == 0)
            throw new Exception("Device is not supported.");

        presentModes = new PresentModeKHR[presentModeCount];
        fixed (PresentModeKHR* presentPtr = presentModes)
            _surfaceExt.GetPhysicalDeviceSurfacePresentModes(device, _surface, &presentModeCount, presentPtr);

        // TODO: Devices NEED to check to make sure VK is supported, as well as ranking best device to worst, so that
        // the best is picked in almost all cases.
        return new VkPhysicalDevice()
        {
            Device = device,
            QueueFamilyIndices = indices,
            
            SurfaceCapabilities = surfaceCapabilities,
            SupportedFormats = surfaceFormats,
            SupportedPresentModes = presentModes
        };
    }

    public VkDevice CreateDevice(VkPhysicalDevice pDevice)
    {
        PieLog.Log(LogType.Verbose, "Creating logical device.");
        
        if (!pDevice.QueueFamilyIndices.IsComplete)
            throw new Exception("Incomplete queue families!");

        HashSet<uint> uniqueQueueFamilies = new HashSet<uint>()
        {
            pDevice.QueueFamilyIndices.GraphicsQueue!.Value,
            pDevice.QueueFamilyIndices.PresentQueue!.Value
        };

        DeviceQueueCreateInfo[] queueCreateInfos = new DeviceQueueCreateInfo[uniqueQueueFamilies.Count];

        float queuePriority = 1.0f;
        
        int i = 0;
        foreach (uint queueFamily in uniqueQueueFamilies)
        {
            queueCreateInfos[i++] = new DeviceQueueCreateInfo()
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = queueFamily,
                QueueCount = 1,
                PQueuePriorities = &queuePriority,
            };
        }

        PhysicalDeviceFeatures features = new PhysicalDeviceFeatures();
        
        PhysicalDeviceDynamicRenderingFeatures dynamicRenderingFeatures = new PhysicalDeviceDynamicRenderingFeatures()
        {
            SType = StructureType.PhysicalDeviceDynamicRenderingFeatures,
            DynamicRendering = true
        };

        using PinnedStringArray layers = new PinnedStringArray(_layers);
        using PinnedStringArray deviceExtensions = new PinnedStringArray(_deviceExtensions);

        Device device;

        fixed (DeviceQueueCreateInfo* queuePtr = queueCreateInfos)
        {
            DeviceCreateInfo deviceCreateInfo = new DeviceCreateInfo()
            {
                SType = StructureType.DeviceCreateInfo,
                PNext = &dynamicRenderingFeatures,
                
                PQueueCreateInfos = queuePtr,
                QueueCreateInfoCount = (uint) queueCreateInfos.Length,
                
                PEnabledFeatures = &features,
                
                PpEnabledLayerNames = (byte**) layers.Handle,
                EnabledLayerCount = layers.Length,
                
                PpEnabledExtensionNames = (byte**) deviceExtensions.Handle,
                EnabledExtensionCount = deviceExtensions.Length
            };
            
            CheckResult(Vk.CreateDevice(pDevice.Device, &deviceCreateInfo, null, out device));
        }

        PieLog.Log(LogType.Verbose, "Getting swapchain extension.");
        // TODO: Device extensions should be stored per VkDevice, not globally.
        Vk.TryGetDeviceExtension(Instance, device, out SwapchainExt);

        PieLog.Log(LogType.Verbose, "Getting graphics queue.");
        Vk.GetDeviceQueue(device, pDevice.QueueFamilyIndices.GraphicsQueue.Value, 0, out Queue graphicsQueue);
        
        PieLog.Log(LogType.Verbose, "Getting present queue.");
        Vk.GetDeviceQueue(device, pDevice.QueueFamilyIndices.PresentQueue.Value, 0, out Queue presentQueue);
        
        PieLog.Log(LogType.Verbose, "Creating command pool.");

        CommandPoolCreateInfo poolCreateInfo = new CommandPoolCreateInfo()
        {
            SType = StructureType.CommandPoolCreateInfo,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
            QueueFamilyIndex = pDevice.QueueFamilyIndices.GraphicsQueue.Value
        };
        
        CheckResult(Vk.CreateCommandPool(device, &poolCreateInfo, null, out CommandPool pool));

        return new VkDevice()
        {
            Device = device,
            
            GraphicsQueue = graphicsQueue,
            PresentQueue = presentQueue,
            
            QueueFamilyIndices = pDevice.QueueFamilyIndices,
            
            CommandPool = pool
        };
    }

    public VkSwapchain CreateSwapchain(VkDevice device, SurfaceFormatKHR surfaceFormat, PresentModeKHR presentMode, Extent2D extent, uint imageCount, SurfaceTransformFlagsKHR transform)
    {
        PieLog.Log(LogType.Verbose, "Creating swapchain.");
        
        SwapchainCreateInfoKHR swapchainInfo = new SwapchainCreateInfoKHR()
        {
            SType = StructureType.SwapchainCreateInfoKhr,

            Surface = _surface,

            MinImageCount = imageCount,
            ImageFormat = surfaceFormat.Format,
            ImageColorSpace = surfaceFormat.ColorSpace,
            ImageExtent = extent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            
            PreTransform = transform,
            
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            
            PresentMode = presentMode,
            Clipped = true
        };
        
        uint* queueFamilyIndices = stackalloc uint[]
            { device.QueueFamilyIndices.GraphicsQueue!.Value, device.QueueFamilyIndices.PresentQueue!.Value };

        if (device.QueueFamilyIndices.GraphicsQueue == device.QueueFamilyIndices.PresentQueue)
            swapchainInfo.ImageSharingMode = SharingMode.Exclusive;
        else
        {
            swapchainInfo.ImageSharingMode = SharingMode.Concurrent;
            swapchainInfo.QueueFamilyIndexCount = 2;
            swapchainInfo.PQueueFamilyIndices = queueFamilyIndices;
        }
        
        CheckResult(SwapchainExt.CreateSwapchain(device.Device, &swapchainInfo, null, out SwapchainKHR swapchain));
        
        PieLog.Log(LogType.Verbose, "Fetching swapchain images.");

        uint sImageCount;
        SwapchainExt.GetSwapchainImages(device.Device, swapchain, &sImageCount, null);

        Image[] images = new Image[sImageCount];
        fixed (Image* imagePtr = images)
            SwapchainExt.GetSwapchainImages(device.Device, swapchain, &sImageCount, imagePtr);

        return new VkSwapchain()
        {
            Swapchain = swapchain,
            Images = images
        };
    }

    public CommandBuffer CreateCommandBuffer(VkDevice device, CommandBufferLevel level)
    {
        PieLog.Log(LogType.Verbose, "Creating command buffer.");
        
        CommandBufferAllocateInfo allocInfo = new CommandBufferAllocateInfo()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = device.CommandPool,
            Level = level,
            CommandBufferCount = 1
        };
        
        CheckResult(Vk.AllocateCommandBuffers(device.Device, &allocInfo, out CommandBuffer buffer));

        return buffer;
    }

    public void CommandBufferBegin(CommandBuffer buffer, RenderingInfo renderingInfo, Image swapchainImage)
    {
        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo()
        {
            SType = StructureType.CommandBufferBeginInfo
        };
        
        CheckResult(Vk.BeginCommandBuffer(buffer, &beginInfo));

        ImageMemoryBarrier memoryBarrier = new ImageMemoryBarrier()
        {
            SType = StructureType.ImageMemoryBarrier,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit,
            OldLayout = ImageLayout.Undefined,
            NewLayout = ImageLayout.ColorAttachmentOptimal,
            Image = swapchainImage,
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseMipLevel = 0,
                LevelCount = 1,
                BaseArrayLayer = 0,
                LayerCount = 1
            }
        };

        Vk.CmdPipelineBarrier(buffer, PipelineStageFlags.TopOfPipeBit, PipelineStageFlags.ColorAttachmentOutputBit,
            DependencyFlags.None, 0, null, 0, null, 1, &memoryBarrier);
        
        Vk.CmdBeginRendering(buffer, &renderingInfo);
    }

    public void CommandBufferEnd(CommandBuffer buffer, Image swapchainImage)
    {
        Vk.CmdEndRendering(buffer);

        ImageMemoryBarrier memoryBarrier = new ImageMemoryBarrier()
        {
            SType = StructureType.ImageMemoryBarrier,
            SrcAccessMask = AccessFlags.ColorAttachmentWriteBit,
            OldLayout = ImageLayout.ColorAttachmentOptimal,
            NewLayout = ImageLayout.PresentSrcKhr,
            Image = swapchainImage,
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseMipLevel = 0,
                LevelCount = 1,
                BaseArrayLayer = 0,
                LayerCount = 1
            }
        };

        Vk.CmdPipelineBarrier(buffer, PipelineStageFlags.ColorAttachmentOutputBit, PipelineStageFlags.BottomOfPipeBit,
            DependencyFlags.None, 0, null, 0, null, 1, &memoryBarrier);
        
        CheckResult(Vk.EndCommandBuffer(buffer));
    }

    public void SwapchainPresent(VkDevice device, VkSwapchain swapchain, CommandBuffer buffer, uint imageIndex,
        Fence fence, Semaphore signalSemaphore, Semaphore waitSemaphore)
    {
        PipelineStageFlags waitStage = PipelineStageFlags.ColorAttachmentOutputBit;

        SubmitInfo submitInfo = new SubmitInfo()
        {
            SType = StructureType.SubmitInfo,

            WaitSemaphoreCount = 1,
            PWaitSemaphores = &waitSemaphore,
            PWaitDstStageMask = &waitStage,

            CommandBufferCount = 1,
            PCommandBuffers = &buffer,

            SignalSemaphoreCount = 1,
            PSignalSemaphores = &signalSemaphore
        };
        
        CheckResult(Vk.QueueSubmit(device.GraphicsQueue, 1, &submitInfo, fence));

        PresentInfoKHR presentInfo = new PresentInfoKHR()
        {
            SType = StructureType.PresentInfoKhr,

            WaitSemaphoreCount = 1,
            PWaitSemaphores = &signalSemaphore,

            SwapchainCount = 1,
            PSwapchains = &swapchain.Swapchain,
            PImageIndices = &imageIndex
        };

        SwapchainExt.QueuePresent(device.PresentQueue, &presentInfo);
    }

    public void DestroyDevice(VkDevice device)
    {
        Vk.DestroyCommandPool(device.Device, device.CommandPool, null);
        Vk.DestroyDevice(device.Device, null);
    }

    public void DestroySwapchain(VkDevice device, VkSwapchain swapchain)
    {
        SwapchainExt.DestroySwapchain(device.Device, swapchain.Swapchain, null);
    }

    public void Dispose()
    {
        SwapchainExt.Dispose();
        
        _surfaceExt.DestroySurface(Instance, _surface, null);
        _surfaceExt.Dispose();
        
        if (_debugUtils != null)
        {
            _debugUtils.DestroyDebugUtilsMessenger(Instance, _debugMessenger, null);
            _debugUtils.Dispose();
        }
        
        Vk.DestroyInstance(Instance, null);
    }

    public static void CheckResult(Result result)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan operation failed. Result: {result}");
    }
    
    private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageseverity,
        DebugUtilsMessageTypeFlagsEXT messagetypes, DebugUtilsMessengerCallbackDataEXT* pcallbackdata, void* puserdata)
    {
        LogType type = messageseverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.None => LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => LogType.Warning,
            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => LogType.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(messageseverity), messageseverity, null)
        };
        
        PieLog.Log(type, $"{messagetypes} | " + Marshal.PtrToStringAnsi((nint) pcallbackdata->PMessage));

        return Vk.False;
    }

    public struct QueueFamilyIndices
    {
        public uint? GraphicsQueue;
        public uint? PresentQueue;

        public bool IsComplete => GraphicsQueue.HasValue && PresentQueue.HasValue;
    }

    public struct VkPhysicalDevice
    {
        public PhysicalDevice Device;
        
        public QueueFamilyIndices QueueFamilyIndices;

        public SurfaceCapabilitiesKHR SurfaceCapabilities;
        public SurfaceFormatKHR[] SupportedFormats;
        public PresentModeKHR[] SupportedPresentModes;
    }

    public struct VkDevice
    {
        public Device Device;
        
        public Queue GraphicsQueue;
        public Queue PresentQueue;

        public QueueFamilyIndices QueueFamilyIndices;

        public CommandPool CommandPool;
    }

    public struct VkSwapchain
    {
        public SwapchainKHR Swapchain;
        public Image[] Images;
    }
}