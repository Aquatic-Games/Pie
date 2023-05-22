namespace Pie.ShaderCompiler.Spirv
{
    public enum spvc_result
    {
        SPVC_SUCCESS = 0,
        SPVC_ERROR_INVALID_SPIRV = -1,
        SPVC_ERROR_UNSUPPORTED_SPIRV = -2,
        SPVC_ERROR_OUT_OF_MEMORY = -3,
        SPVC_ERROR_INVALID_ARGUMENT = -4,
        SPVC_ERROR_INT_MAX = 0x7fffffff,
    }
}
