namespace Pie.Shaderc.Native;

[NativeTypeName("int")]
public enum shaderc_env_version : uint
{
    shaderc_env_version_vulkan_1_0 = ((1U << 22)),
    shaderc_env_version_vulkan_1_1 = ((1U << 22) | (1 << 12)),
    shaderc_env_version_vulkan_1_2 = ((1U << 22) | (2 << 12)),
    shaderc_env_version_vulkan_1_3 = ((1U << 22) | (3 << 12)),
    shaderc_env_version_opengl_4_5 = 450,
    shaderc_env_version_webgpu,
}
