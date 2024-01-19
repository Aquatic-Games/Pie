namespace Pie.Shaderc.Native;

public enum shaderc_target_env
{
    shaderc_target_env_vulkan,
    shaderc_target_env_opengl,
    shaderc_target_env_opengl_compat,
    shaderc_target_env_webgpu,
    shaderc_target_env_default = shaderc_target_env_vulkan,
}
