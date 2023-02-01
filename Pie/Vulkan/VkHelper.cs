using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using VFramebuffer = Silk.NET.Vulkan.Framebuffer;

namespace Pie.Vulkan;

public static unsafe class VkHelper
{
    public static Vk VK;
    public static Instance Instance;
    public static Device Device;
    public static CommandPool CommandPool;
    public static CommandBuffer CommandBuffer;
    public static uint CurrentImage;
    public static Extent2D CurrentExtent;

    public static Semaphore ImageAvailableSemaphore;
    public static Semaphore RenderFinishedSemaphore;
    public static Fence InFrameFence;

    public static Queue DeviceQueue;
    public static Queue PresentQueue;
    public static SwapchainKHR Swapchain;

    private static ExtDebugUtils _debugUtilsExt;
    private static DebugUtilsMessengerEXT _debugMessenger;

    private static KhrSurface _surfaceExt;
    private static SurfaceKHR _surface;

    private static KhrSwapchain _swapchainExt;
    private static Image[] _images;
    private static ImageView[] _imageViews;

    private static RenderPass _renderPass;
    private static VFramebuffer[] _framebuffers;

    private static bool _isInRenderPass;
    private static bool _isInFrame;

    private static PhysicalDeviceMemoryProperties _physicalProperties;

    #region Vulkan initialization
    
    public static void InitVulkan(string[] instanceExtensions, bool debug)
    {
        VK = Vk.GetApi();
        
        ApplicationInfo appInfo = new ApplicationInfo();
        appInfo.SType = StructureType.ApplicationInfo;
        appInfo.PApplicationName = (byte*) SilkMarshal.StringToPtr("Pie Application");
        appInfo.PEngineName = (byte*) SilkMarshal.StringToPtr("Pie");
        appInfo.ApplicationVersion = Vk.MakeVersion(1, 0, 0);
        appInfo.EngineVersion = Vk.MakeVersion(1, 0, 0);
        appInfo.ApiVersion = Vk.Version10;

        InstanceCreateInfo createInfo = new InstanceCreateInfo();
        createInfo.SType = StructureType.InstanceCreateInfo;
        createInfo.PApplicationInfo = &appInfo;

        // Check for validation layers
        if (debug)
        {
            Array.Resize(ref instanceExtensions, instanceExtensions.Length + 1);
            instanceExtensions[^1] = ExtDebugUtils.ExtensionName;

            bool validationSupported = false;

            uint layers;
            VK.EnumerateInstanceLayerProperties(&layers, null);

            LayerProperties[] layerProperties = new LayerProperties[layers];
            fixed (LayerProperties* props = layerProperties)
                VK.EnumerateInstanceLayerProperties(&layers, props);
            
            string[] validationLayers = new[] { "VK_LAYER_KHRONOS_validation" };

            foreach (string layer in validationLayers)
            {
                foreach (LayerProperties property in layerProperties)
                {
                    if (Marshal.PtrToStringAnsi((IntPtr) property.LayerName) == layer)
                    {
                        validationSupported = true;
                        break;
                    }
                }
            }

            if (validationSupported)
            {
                byte** validations = (byte**) SilkMarshal.StringArrayToPtr(validationLayers);
                createInfo.EnabledLayerCount = (uint) validationLayers.Length;
                createInfo.PpEnabledLayerNames = validations;
            }
            else
                Logging.Log(LogType.Error, "Debug requested, but validation layers not supported.");
        }

        createInfo.EnabledExtensionCount = (uint) instanceExtensions.Length;
        createInfo.PpEnabledExtensionNames = (byte**) SilkMarshal.StringArrayToPtr(instanceExtensions);

        Result result;
        if ((result = VK.CreateInstance(&createInfo, null, out Instance)) != Result.Success)
            throw new PieException("Failed to create Vulkan instance: " + result);

        Console.WriteLine("VK initialized.");
    }

    public static void CreateDebugMessenger(DebugUtilsMessengerCallbackFunctionEXT debugCallback)
    {
        DebugUtilsMessengerCreateInfoEXT messengerCreateInfo = new DebugUtilsMessengerCreateInfoEXT();
        messengerCreateInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
        messengerCreateInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                              DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                              DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                              DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
        messengerCreateInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                                          DebugUtilsMessageTypeFlagsEXT.ValidationBitExt |
                                          DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt;
        messengerCreateInfo.PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(debugCallback);

        VK.TryGetInstanceExtension(Instance, out _debugUtilsExt);
        _debugUtilsExt.CreateDebugUtilsMessenger(Instance, &messengerCreateInfo, null, out _debugMessenger);
    }
    
    #endregion
    
    #region Device creation

    public static PhysicalDevice? ChooseSuitableDevice(string[] extensions, in SurfaceKHR surface, out QueueFamilyIndices queueFamilyIndices,
        out SwapchainSupportDetails swapchainSupportDetails)
    {
        VK.TryGetInstanceExtension(Instance, out _surfaceExt);
        _surface = surface;
        
        uint deviceCount;
        VK.EnumeratePhysicalDevices(Instance, &deviceCount, null);

        queueFamilyIndices = default;
        swapchainSupportDetails = default;

        if (deviceCount == 0)
            return null;

        PhysicalDevice[] devices = new PhysicalDevice[deviceCount];
        fixed (PhysicalDevice* devs = devices)
            VK.EnumeratePhysicalDevices(Instance, &deviceCount, devs);
        
        foreach (PhysicalDevice dv in devices)
        {
            if (CheckDeviceSuitability(dv, extensions, out queueFamilyIndices, out swapchainSupportDetails))
                return dv;
        }

        return null;
    }

    // TODO: Clean this up so that debug mode, instance extensions, device extensions, and validation layers are all globally available.
    public static void CreateLogicalDevice(in PhysicalDevice physicalDevice, string[] extensions, string[] validationLayers, bool debug, in QueueFamilyIndices indices)
    {
        HashSet<uint> uniqueQueueFamilies = new HashSet<uint>()
        {
            indices.GraphicsFamily!.Value,
            indices.PresentFamily!.Value
        };

        DeviceQueueCreateInfo[] queueCreateInfos = new DeviceQueueCreateInfo[uniqueQueueFamilies.Count];

        float priority = 1.0f;
        int i = 0;
        foreach (uint queueFamily in uniqueQueueFamilies)
        {
            DeviceQueueCreateInfo info = new DeviceQueueCreateInfo();
            info.SType = StructureType.DeviceQueueCreateInfo;
            info.QueueFamilyIndex = queueFamily;
            info.QueueCount = 1;
            info.PQueuePriorities = &priority;
            queueCreateInfos[i++] = info;
        }

        PhysicalDeviceFeatures features = new PhysicalDeviceFeatures();

        fixed (DeviceQueueCreateInfo* infos = queueCreateInfos)
        {
            DeviceCreateInfo createInfo = new DeviceCreateInfo();
            createInfo.SType = StructureType.DeviceCreateInfo;
            createInfo.PQueueCreateInfos = infos;
            createInfo.QueueCreateInfoCount = (uint) queueCreateInfos.Length;
            createInfo.PEnabledFeatures = &features;

            createInfo.EnabledExtensionCount = (uint) extensions.Length;
            createInfo.PpEnabledExtensionNames = (byte**) SilkMarshal.StringArrayToPtr(extensions);

            if (debug)
            {
                byte** validationPtr = (byte**) SilkMarshal.StringArrayToPtr(validationLayers);
                createInfo.EnabledLayerCount = (uint) validationLayers.Length;
                createInfo.PpEnabledLayerNames = validationPtr;
            }
            else
                createInfo.EnabledLayerCount = 0;

            Result result;
            if ((result = VK.CreateDevice(physicalDevice, &createInfo, null, out Device)) != Result.Success)
                throw new PieException("Failed to create a logical device: " + result);
        }
        
        VK.GetDeviceQueue(Device, indices.GraphicsFamily!.Value, 0, out DeviceQueue);
        VK.GetDeviceQueue(Device, indices.PresentFamily!.Value, 0, out PresentQueue);
        
        VK.GetPhysicalDeviceMemoryProperties(physicalDevice, out _physicalProperties);
        
        Console.WriteLine("Logical device & queues created.");
    }

    public static bool CheckDeviceSuitability(in PhysicalDevice device, string[] extensions,
        out QueueFamilyIndices queueFamilyIndices, out SwapchainSupportDetails swapchainSupportDetails)
    {
        PhysicalDeviceProperties properties;
        PhysicalDeviceFeatures features;

        VK.GetPhysicalDeviceProperties(device, &properties);
        VK.GetPhysicalDeviceFeatures(device, &features);

        queueFamilyIndices = default;
        swapchainSupportDetails = default;

        bool extensionsSupported = CheckDeviceSupportsExtensions(device, extensions);
        if (!extensionsSupported)
            return false;

        swapchainSupportDetails = QuerySwapchainSupport(device);
        bool swapchainAdequate = swapchainSupportDetails.Formats.Length > 0 &&
                                 swapchainSupportDetails.PresentModes.Length > 0;

        queueFamilyIndices = FindQueueFamilies(device);

        return swapchainAdequate && queueFamilyIndices.Complete;
    }

    public static bool CheckDeviceSupportsExtensions(in PhysicalDevice device, string[] extensions)
    {
        uint extensionCount;
        VK.EnumerateDeviceExtensionProperties(device, (byte*) 0, &extensionCount, null);

        ExtensionProperties[] properties = new ExtensionProperties[extensionCount];
        fixed (ExtensionProperties* property = properties)
            VK.EnumerateDeviceExtensionProperties(device, (byte*) 0, &extensionCount, property);

        int numMatches = 0;
        foreach (ExtensionProperties property in properties)
        {
            foreach (string extension in extensions)
            {
                Console.WriteLine(Marshal.PtrToStringAnsi((IntPtr) property.ExtensionName));
                
                if (extension == Marshal.PtrToStringAnsi((IntPtr) property.ExtensionName))
                    numMatches++;
            }
        }

        return numMatches == extensions.Length;
    }

    public static QueueFamilyIndices FindQueueFamilies(in PhysicalDevice device)
    {
        QueueFamilyIndices indices = new QueueFamilyIndices();

        uint queueFamilyCount;
        VK.GetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, null);

        QueueFamilyProperties[] families = new QueueFamilyProperties[queueFamilyCount];
        fixed (QueueFamilyProperties* props = families)
            VK.GetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, props);

        for (uint i = 0; i < families.Length; i++)
        {
            if ((families[i].QueueFlags & QueueFlags.GraphicsBit) == QueueFlags.GraphicsBit)
                indices.GraphicsFamily = i;

            Bool32 present;
            _surfaceExt.GetPhysicalDeviceSurfaceSupport(device, i, _surface, out present);
            if (present)
                indices.PresentFamily = i;

            if (indices.Complete)
                break;
        }

        return indices;
    }

    public static SwapchainSupportDetails QuerySwapchainSupport(in PhysicalDevice device)
    {
        SwapchainSupportDetails details = new SwapchainSupportDetails();

        _surfaceExt.GetPhysicalDeviceSurfaceCapabilities(device, _surface, &details.Capabilities);

        uint formatCount;
        _surfaceExt.GetPhysicalDeviceSurfaceFormats(device, _surface, &formatCount, null);
        details.Formats = new SurfaceFormatKHR[formatCount];
        fixed (SurfaceFormatKHR* forms = details.Formats)
            _surfaceExt.GetPhysicalDeviceSurfaceFormats(device, _surface, &formatCount, forms);
        
        uint presentModeCount;
        _surfaceExt.GetPhysicalDeviceSurfacePresentModes(device, _surface, &presentModeCount, null);
        details.PresentModes = new PresentModeKHR[presentModeCount];
        fixed (PresentModeKHR* modes = details.PresentModes)
            _surfaceExt.GetPhysicalDeviceSurfacePresentModes(device, _surface, &presentModeCount, modes);

        return details;
    }
    
    #endregion

    #region Swapchain

    public static void CreateSwapchain(in SwapchainSupportDetails details, in QueueFamilyIndices indices, in Size size)
    {
        const Silk.NET.Vulkan.Format format = Silk.NET.Vulkan.Format.B8G8R8A8Unorm;
        const ColorSpaceKHR colorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr;

        Result result;

        if (!CheckDeviceSupportsFormat(details, format, colorSpace))
            throw new PieException("Given swapchain format is not supported on this graphics device.");

        // TODO: Disable vsync?
        const PresentModeKHR presentMode = PresentModeKHR.FifoKhr;

        Extent2D swapchainSize = new Extent2D((uint) size.Width, (uint) size.Height);
        CurrentExtent = swapchainSize;
        
        Console.WriteLine(details.Capabilities.CurrentExtent);

        uint imageCount = details.Capabilities.MinImageCount + 1;
        if (details.Capabilities.MaxImageCount > 0 && imageCount > details.Capabilities.MaxImageCount)
            imageCount = details.Capabilities.MaxImageCount;

        uint[] queueFamilyIndices = { indices.GraphicsFamily!.Value, indices.PresentFamily!.Value };

        fixed (uint* queuePtr = queueFamilyIndices)
        {
            SwapchainCreateInfoKHR createInfo = new SwapchainCreateInfoKHR();
            createInfo.SType = StructureType.SwapchainCreateInfoKhr;
            createInfo.Surface = _surface;
            createInfo.MinImageCount = imageCount;
            createInfo.ImageFormat = format;
            createInfo.ImageColorSpace = colorSpace;
            createInfo.ImageExtent = swapchainSize;
            createInfo.ImageArrayLayers = 1;
            createInfo.ImageUsage = ImageUsageFlags.ColorAttachmentBit;
            createInfo.PreTransform = details.Capabilities.CurrentTransform;
            createInfo.CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr;
            createInfo.PresentMode = presentMode;
            createInfo.Clipped = true;

            if (indices.GraphicsFamily != indices.PresentFamily)
            {
                createInfo.ImageSharingMode = SharingMode.Concurrent;
                createInfo.QueueFamilyIndexCount = 2;
                createInfo.PQueueFamilyIndices = queuePtr;
            }
            else
            {
                createInfo.ImageSharingMode = SharingMode.Exclusive;
                createInfo.QueueFamilyIndexCount = 0;
                createInfo.PQueueFamilyIndices = null;
            }

            VK.TryGetDeviceExtension(Instance, Device, out _swapchainExt);

            if ((result = _swapchainExt.CreateSwapchain(Device, &createInfo, null, out Swapchain)) != Result.Success)
                throw new PieException("Failed to create swapchain: " + result);
        }

        uint images;
        _swapchainExt.GetSwapchainImages(Device, Swapchain, &images, null);
        _images = new Image[images];
        fixed (Image* image = _images)
            _swapchainExt.GetSwapchainImages(Device, Swapchain, &images, image);

        _imageViews = new ImageView[_images.Length];
        for (int i = 0; i < _images.Length; i++)
        {
            ImageViewCreateInfo info = new ImageViewCreateInfo();
            info.SType = StructureType.ImageViewCreateInfo;
            info.Image = _images[i];
            info.ViewType = ImageViewType.Type2D;
            info.Format = format;
            info.Components.R = ComponentSwizzle.Identity;
            info.Components.G = ComponentSwizzle.Identity;
            info.Components.B = ComponentSwizzle.Identity;
            info.Components.A = ComponentSwizzle.Identity;
            info.SubresourceRange.AspectMask = ImageAspectFlags.ColorBit;
            info.SubresourceRange.BaseMipLevel = 0;
            info.SubresourceRange.LevelCount = 1;
            info.SubresourceRange.BaseArrayLayer = 0;
            info.SubresourceRange.LayerCount = 1;

            ImageView view;
            if ((result = VK.CreateImageView(Device, &info, null, out view)) != Result.Success)
                throw new PieException("Failed to create image view: " + result);

            _imageViews[i] = view;
        }
        
        Console.WriteLine("Swapchain created.");
        
        CreateRenderPass(format);
        CreateFramebuffers(size);
        
        Console.WriteLine("Created render pass and framebuffer.");
    }

    public static void CreateRenderPass(Silk.NET.Vulkan.Format format)
    {
        AttachmentDescription colorAttachment = new AttachmentDescription();
        colorAttachment.Format = format;
        colorAttachment.Samples = SampleCountFlags.Count1Bit;
        colorAttachment.LoadOp = AttachmentLoadOp.Clear;
        colorAttachment.StoreOp = AttachmentStoreOp.Store;
        colorAttachment.StencilLoadOp = AttachmentLoadOp.DontCare;
        colorAttachment.StencilStoreOp = AttachmentStoreOp.DontCare;
        colorAttachment.InitialLayout = ImageLayout.Undefined;
        colorAttachment.FinalLayout = ImageLayout.PresentSrcKhr;

        AttachmentReference colorAttachmentRef = new AttachmentReference();
        colorAttachmentRef.Attachment = 0;
        colorAttachmentRef.Layout = ImageLayout.ColorAttachmentOptimal;

        SubpassDescription subpass = new SubpassDescription();
        subpass.PipelineBindPoint = PipelineBindPoint.Graphics;
        subpass.ColorAttachmentCount = 1;
        subpass.PColorAttachments = &colorAttachmentRef;

        SubpassDependency dependency = new SubpassDependency();
        dependency.SrcSubpass = Vk.SubpassExternal;
        dependency.DstSubpass = 0;

        dependency.SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit;
        dependency.SrcAccessMask = AccessFlags.None;
        dependency.DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit;
        dependency.DstAccessMask = AccessFlags.ColorAttachmentWriteBit;

        RenderPassCreateInfo renderPassInfo = new RenderPassCreateInfo();
        renderPassInfo.SType = StructureType.RenderPassCreateInfo;
        renderPassInfo.AttachmentCount = 1;
        renderPassInfo.PAttachments = &colorAttachment;
        renderPassInfo.SubpassCount = 1;
        renderPassInfo.PSubpasses = &subpass;
        renderPassInfo.DependencyCount = 1;
        renderPassInfo.PDependencies = &dependency;

        Result result;
        if ((result = VK.CreateRenderPass(Device, &renderPassInfo, null, out _renderPass)) != Result.Success)
            throw new PieException("Failed to create render pass: " + result);
    }

    public static void CreateFramebuffers(Size size)
    {
        _framebuffers = new VFramebuffer[_imageViews.Length];
        Result result;

        for (int i = 0; i < _imageViews.Length; i++)
        {
            ImageView imageView = _imageViews[i];

            FramebufferCreateInfo framebufferInfo = new FramebufferCreateInfo();
            framebufferInfo.SType = StructureType.FramebufferCreateInfo;
            framebufferInfo.RenderPass = _renderPass;
            framebufferInfo.AttachmentCount = 1;
            framebufferInfo.PAttachments = &imageView;
            framebufferInfo.Width = (uint) size.Width;
            framebufferInfo.Height = (uint) size.Height;
            framebufferInfo.Layers = 1;

            if ((result = VK.CreateFramebuffer(Device, &framebufferInfo, null, out _framebuffers[i])) != Result.Success)
                throw new PieException("Failed to create framebuffer: " + result);
        }
    }

    public static bool CheckDeviceSupportsFormat(in SwapchainSupportDetails details, in Silk.NET.Vulkan.Format desiredFormat, in ColorSpaceKHR colorSpace)
    {
        foreach (SurfaceFormatKHR format in details.Formats)
        {
            if (format.Format == desiredFormat && format.ColorSpace == colorSpace)
                return true;
        }

        return false;
    }

    #endregion
    
    #region Command pools & sync

    public static void CreateCommandPoolAndBuffer(in QueueFamilyIndices indices)
    {
        CommandPoolCreateInfo poolInfo = new CommandPoolCreateInfo();
        poolInfo.SType = StructureType.CommandPoolCreateInfo;
        poolInfo.Flags = CommandPoolCreateFlags.ResetCommandBufferBit;
        poolInfo.QueueFamilyIndex = indices.GraphicsFamily!.Value;

        Result result;
        if ((result = VK.CreateCommandPool(Device, &poolInfo, null, out CommandPool)) != Result.Success)
            throw new PieException("Failed to create command pool: " + result);

        CommandBufferAllocateInfo allocInfo = new CommandBufferAllocateInfo();
        allocInfo.SType = StructureType.CommandBufferAllocateInfo;
        allocInfo.CommandPool = CommandPool;
        allocInfo.Level = CommandBufferLevel.Primary;
        allocInfo.CommandBufferCount = 1;

        if ((result = VK.AllocateCommandBuffers(Device, &allocInfo, out CommandBuffer)) != Result.Success)
            throw new Exception("Failed to allocate command buffer: " + result);
        
        Console.WriteLine("Command pool + buffer created.");
    }

    public static void CreateSyncObjects()
    {
        SemaphoreCreateInfo sci = new SemaphoreCreateInfo();
        sci.SType = StructureType.SemaphoreCreateInfo;

        FenceCreateInfo fci = new FenceCreateInfo();
        fci.SType = StructureType.FenceCreateInfo;

        Result result;
        if ((result = VK.CreateSemaphore(Device, &sci, null, out ImageAvailableSemaphore)) != Result.Success)
            throw new PieException("Failed to create image semaphore: " + result);
        if ((result = VK.CreateSemaphore(Device, &sci, null, out RenderFinishedSemaphore)) != Result.Success)
            throw new PieException("Failed to create render semaphore: " + result);
        if ((result = VK.CreateFence(Device, &fci, null, out InFrameFence)) != Result.Success)
            throw new PieException("Failed to create fence: " + result);
        
        Console.WriteLine("Sync objects created.");
    }
    
    #endregion

    public static uint GetMemoryTypeIndex(uint typeBits, MemoryPropertyFlags properties)
    {
        for (uint i = 0; i < _physicalProperties.MemoryTypeCount; i++)
        {
            if ((typeBits & 1) == 1 && (_physicalProperties.MemoryTypes[(int) i].PropertyFlags & properties) == properties)
                return i;
            typeBits >>= 1;
        }

        throw new PieException("Could not find a suitable memory type.");
    }

    public static CommandBuffer CreateCommandBuffer(bool begin)
    {
        CommandBuffer cmdBuffer;

        CommandBufferAllocateInfo allocInfo = new CommandBufferAllocateInfo();
        allocInfo.SType = StructureType.CommandBufferAllocateInfo;
        allocInfo.CommandPool = CommandPool;
        allocInfo.Level = CommandBufferLevel.Primary;
        allocInfo.CommandBufferCount = 1;

        Result result;
        if ((result = VK.AllocateCommandBuffers(Device, &allocInfo, &cmdBuffer)) != Result.Success)
            throw new PieException("Failed to create comamnd buffer: " + result);

        if (begin)
        {
            CommandBufferBeginInfo cbbi = new CommandBufferBeginInfo();
            cbbi.SType = StructureType.CommandBufferBeginInfo;
            cbbi.Flags = CommandBufferUsageFlags.OneTimeSubmitBit;
            if ((result = VK.BeginCommandBuffer(cmdBuffer, &cbbi)) != Result.Success)
                throw new PieException("Failed to begin command buffer: " + result);
        }

        return cmdBuffer;
    }

    public static void FlushAndDeleteCommandBuffer(CommandBuffer buffer)
    {
        Result result;
        if ((result = VK.EndCommandBuffer(buffer)) != Result.Success)
            throw new PieException("Failed to end command buffer:" + result);

        SubmitInfo submitInfo = new SubmitInfo();
        submitInfo.SType = StructureType.SubmitInfo;
        submitInfo.CommandBufferCount = 1;
        submitInfo.PCommandBuffers = &buffer;

        FenceCreateInfo fci = new FenceCreateInfo();
        fci.SType = StructureType.FenceCreateInfo;
        fci.Flags = FenceCreateFlags.None;

        Fence fence;
        if ((result = VK.CreateFence(Device, &fci, null, &fence)) != Result.Success)
            throw new PieException("Failed to create fence: " + result);

        if ((result = VK.QueueSubmit(DeviceQueue, 1, &submitInfo, fence)) != Result.Success)
            throw new PieException("Failed to submit queue: " + result);

        VK.WaitForFences(Device, 1, &fence, true, ulong.MaxValue);
        
        VK.DestroyFence(Device, fence, null);
        VK.FreeCommandBuffers(Device, CommandPool, 1, &buffer);
    }

    public static void BeginNewPass(ClearValue clearValue)
    {
        EndRenderPassIfNotEnded();
        NewFrameIfPresented();

        _isInRenderPass = true;

        ClearColorValue value = new ClearColorValue(1.0f, 0.0f, 1.0f, 1.0f);

        ImageSubresourceRange range = new ImageSubresourceRange();
        range.AspectMask = ImageAspectFlags.ColorBit;
        range.LayerCount = 1;
        range.BaseArrayLayer = 0;
        range.LevelCount = 1;
        range.BaseMipLevel = 0;
        
        //VK.CmdClearColorImage(CommandBuffer, _images[CurrentImage], ImageLayout.TransferDstOptimal, &value, 1,
        //    &range);

        RenderPassBeginInfo renderPassInfo = new RenderPassBeginInfo();
        renderPassInfo.SType = StructureType.RenderPassBeginInfo;
        renderPassInfo.RenderPass = _renderPass;
        renderPassInfo.Framebuffer = _framebuffers[CurrentImage];
        renderPassInfo.RenderArea = new Rect2D(new Offset2D(0, 0), CurrentExtent);
        renderPassInfo.ClearValueCount = 1;
        renderPassInfo.PClearValues = &clearValue;

        VK.CmdBeginRenderPass(CommandBuffer, &renderPassInfo, SubpassContents.Inline);
    }

    public static bool EndRenderPassIfNotEnded()
    {
        if (!_isInRenderPass)
            return false;
        
        VK.CmdEndRenderPass(CommandBuffer);

        _isInRenderPass = false;
        return true;
    }

    public static bool NewFrameIfPresented()
    {
        if (_isInFrame)
            return false;
        
        _swapchainExt.AcquireNextImage(Device, Swapchain, ulong.MaxValue, ImageAvailableSemaphore, new Fence(),
            ref CurrentImage);

        VK.ResetCommandBuffer(CommandBuffer, CommandBufferResetFlags.None);

        // One time submit bit because the command buffer is rerecorded every time.
        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo();
        beginInfo.SType = StructureType.CommandBufferBeginInfo;
        beginInfo.Flags = CommandBufferUsageFlags.OneTimeSubmitBit;
        beginInfo.PInheritanceInfo = null;

        Result result;
        if ((result = VK.BeginCommandBuffer(CommandBuffer, &beginInfo)) != Result.Success)
            throw new PieException("Failed to begin command buffer: " + result);

        _isInFrame = true;
        return true;
    }

    public static void Present()
    {
        EndRenderPassIfNotEnded();

        Result result;
        if ((result = VK.EndCommandBuffer(CommandBuffer)) != Result.Success)
            throw new PieException("Failed to end the command buffer: " + result);
        
        SubmitInfo submitInfo = new SubmitInfo();
        submitInfo.SType = StructureType.SubmitInfo;

        Semaphore waitSemaphore = ImageAvailableSemaphore;
        PipelineStageFlags waitFlags = PipelineStageFlags.ColorAttachmentOutputBit;
        CommandBuffer commandBuffer = CommandBuffer;
        Semaphore signalSemaphore = RenderFinishedSemaphore;

        submitInfo.WaitSemaphoreCount = 1;
        submitInfo.PWaitSemaphores = &waitSemaphore;
        submitInfo.PWaitDstStageMask = &waitFlags;
        submitInfo.CommandBufferCount = 1;
        submitInfo.PCommandBuffers = &commandBuffer;
        submitInfo.SignalSemaphoreCount = 1;
        submitInfo.PSignalSemaphores = &signalSemaphore;
        
        if ((result = VK.QueueSubmit(DeviceQueue, 1, &submitInfo, InFrameFence)) != Result.Success)
            throw new PieException("Failed to submit command buffer: " + result);

        PresentInfoKHR presentInfo = new PresentInfoKHR();
        presentInfo.SType = StructureType.PresentInfoKhr;
        presentInfo.WaitSemaphoreCount = 1;
        presentInfo.PWaitSemaphores = &signalSemaphore;

        uint imageIndex = CurrentImage;
        
        SwapchainKHR swapchain = Swapchain;
        presentInfo.SwapchainCount = 1;
        presentInfo.PSwapchains = &swapchain;
        presentInfo.PImageIndices = &imageIndex;
        
        _isInFrame = false;

        _swapchainExt.QueuePresent(PresentQueue, &presentInfo);
    }

    public static void Dispose()
    {
        VK.DeviceWaitIdle(Device);
        
        VK.DestroySemaphore(Device, ImageAvailableSemaphore, null);
        VK.DestroySemaphore(Device, RenderFinishedSemaphore, null);
        VK.DestroyFence(Device, InFrameFence, null);

        VK.DestroyCommandPool(Device, CommandPool, null);
        
        for (int i = 0; i < _framebuffers.Length; i++)
            VK.DestroyFramebuffer(Device, _framebuffers[i], null);
        
        VK.DestroyRenderPass(Device, _renderPass, null);
        
        foreach (ImageView view in _imageViews)
            VK.DestroyImageView(Device, view, null);
        
        _swapchainExt.DestroySwapchain(Device, Swapchain, null);
        
        VK.DestroyDevice(Device, null);
        _surfaceExt.DestroySurface(Instance, _surface, null);
        
        // TODO: Store debug globally
        _debugUtilsExt.DestroyDebugUtilsMessenger(Instance, _debugMessenger, null);
        
        VK.DestroyInstance(Instance, null);
        VK.Dispose();
    }

    public struct QueueFamilyIndices
    {
        public uint? GraphicsFamily;
        public uint? PresentFamily;

        public bool Complete => GraphicsFamily.HasValue && PresentFamily.HasValue;
    }

    public struct SwapchainSupportDetails
    {
        public SurfaceCapabilitiesKHR Capabilities;
        public SurfaceFormatKHR[] Formats;
        public PresentModeKHR[] PresentModes;
    }
}