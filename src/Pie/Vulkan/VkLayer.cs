using System;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;

namespace Pie.Vulkan;

// Acts as a minimal abstraction over vulkan.
public unsafe class VkLayer : IDisposable
{
    public Vk Vk;
    public Instance Instance;

    private ExtDebugUtils _debugUtils;
    private DebugUtilsMessengerEXT _debugMessenger;

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

        string[] layers;

        if (debug)
        {
            Array.Resize(ref instanceExtensions, instanceExtensions.Length + 1);
            instanceExtensions[^1] = ExtDebugUtils.ExtensionName;

            layers = new[]
            {
                "VK_LAYER_KHRONOS_validation"
            };
        }
        else
            layers = Array.Empty<string>();
        
        PieLog.Log(LogType.Verbose, "Instance extensions: " + string.Join(", ", instanceExtensions));
        PieLog.Log(LogType.Verbose, "Layers: " + string.Join(", ", layers));

        using PinnedStringArray instanceExtPtrs = new PinnedStringArray(instanceExtensions);
        using PinnedStringArray layersPtr = new PinnedStringArray(layers);

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
    }

    public void Dispose()
    {
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
}