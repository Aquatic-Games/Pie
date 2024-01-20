namespace Pie.Spirv.Cross.Native;

public enum spvc_msl_shader_variable_format
{
    SPVC_MSL_SHADER_VARIABLE_FORMAT_OTHER = 0,
    SPVC_MSL_SHADER_VARIABLE_FORMAT_UINT8 = 1,
    SPVC_MSL_SHADER_VARIABLE_FORMAT_UINT16 = 2,
    SPVC_MSL_SHADER_VARIABLE_FORMAT_ANY16 = 3,
    SPVC_MSL_SHADER_VARIABLE_FORMAT_ANY32 = 4,
    SPVC_MSL_VERTEX_FORMAT_OTHER = SPVC_MSL_SHADER_VARIABLE_FORMAT_OTHER,
    SPVC_MSL_VERTEX_FORMAT_UINT8 = SPVC_MSL_SHADER_VARIABLE_FORMAT_UINT8,
    SPVC_MSL_VERTEX_FORMAT_UINT16 = SPVC_MSL_SHADER_VARIABLE_FORMAT_UINT16,
    SPVC_MSL_SHADER_INPUT_FORMAT_OTHER = SPVC_MSL_SHADER_VARIABLE_FORMAT_OTHER,
    SPVC_MSL_SHADER_INPUT_FORMAT_UINT8 = SPVC_MSL_SHADER_VARIABLE_FORMAT_UINT8,
    SPVC_MSL_SHADER_INPUT_FORMAT_UINT16 = SPVC_MSL_SHADER_VARIABLE_FORMAT_UINT16,
    SPVC_MSL_SHADER_INPUT_FORMAT_ANY16 = SPVC_MSL_SHADER_VARIABLE_FORMAT_ANY16,
    SPVC_MSL_SHADER_INPUT_FORMAT_ANY32 = SPVC_MSL_SHADER_VARIABLE_FORMAT_ANY32,
    SPVC_MSL_SHADER_INPUT_FORMAT_INT_MAX = 0x7fffffff,
}