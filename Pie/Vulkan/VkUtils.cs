using Silk.NET.Vulkan;

namespace Pie.Vulkan;

internal static class VkUtils
{
    public static void CheckVkResult(Result result)
    {
        if (result != Result.Success)
            throw new PieException("Vulkan result failed: " + result);
    }
}