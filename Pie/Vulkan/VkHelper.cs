using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Pie.Vulkan;

public static unsafe class VkHelper
{
    public static Vk VK;
    public static Instance Instance;
    public static Device Device;

    public static Queue DeviceQueue;
    public static Queue PresentQueue;

    private static ExtDebugUtils _debugUtilsExt;
    private static DebugUtilsMessengerEXT _debugMessenger;

    private static KhrSurface _surfaceExt;
    private static SurfaceKHR _surface;

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

    public static void Dispose()
    {
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