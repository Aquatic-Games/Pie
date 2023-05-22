namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_format_resolution : uint
    {
        SPVC_MSL_FORMAT_RESOLUTION_444 = 0,
        SPVC_MSL_FORMAT_RESOLUTION_422,
        SPVC_MSL_FORMAT_RESOLUTION_420,
        SPVC_MSL_FORMAT_RESOLUTION_INT_MAX = 0x7fffffff,
    }
}
