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

    private ExtDebugUtils _debugUtils;
    private DebugUtilsMessengerEXT _debugMessenger;

    private KhrSurface _surfaceExt;
    private SurfaceKHR _surface;

    private KhrSwapchain _swapchainExt;
    private SwapchainKHR _swapchain;

    public VkLayer(PieVkContext context, bool debug)
    {
        PieLog.Log(LogType.Verbose, "Creating VK layer.");
        
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
            
        // TODO: Devices NEED to check to make sure VK is supported, as well as ranking best device to worst, so that
        // the best is picked in almost all cases.
        return new VkPhysicalDevice()
        {
            Device = device,
            QueueFamilyIndices = indices
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

        using PinnedStringArray layers = new PinnedStringArray(_layers);

        Device device;

        fixed (DeviceQueueCreateInfo* queuePtr = queueCreateInfos)
        {
            DeviceCreateInfo deviceCreateInfo = new DeviceCreateInfo()
            {
                SType = StructureType.DeviceCreateInfo,
                
                PQueueCreateInfos = queuePtr,
                QueueCreateInfoCount = (uint) queueCreateInfos.Length,
                
                PEnabledFeatures = &features,
                
                PpEnabledLayerNames = (byte**) layers.Handle,
                EnabledLayerCount = (uint) _layers.Length
            };
            
            CheckResult(Vk.CreateDevice(pDevice.Device, &deviceCreateInfo, null, out device));
        }

        PieLog.Log(LogType.Verbose, "Getting graphics queue.");
        Vk.GetDeviceQueue(device, pDevice.QueueFamilyIndices.GraphicsQueue.Value, 0, out Queue graphicsQueue);
        
        PieLog.Log(LogType.Verbose, "Getting present queue.");
        Vk.GetDeviceQueue(device, pDevice.QueueFamilyIndices.PresentQueue.Value, 0, out Queue presentQueue);

        return new VkDevice()
        {
            Device = device,
            GraphicsQueue = graphicsQueue,
            PresentQueue = presentQueue
        };
    }

    public void DestroyDevice(VkDevice device)
    {
        Vk.DestroyDevice(device.Device, null);
    }

    public void Dispose()
    {
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
    }

    public struct VkDevice
    {
        public Device Device;
        public Queue GraphicsQueue;
        public Queue PresentQueue;
    }
}