namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_sampler_border_color : uint
    {
        SPVC_MSL_SAMPLER_BORDER_COLOR_TRANSPARENT_BLACK = 0,
        SPVC_MSL_SAMPLER_BORDER_COLOR_OPAQUE_BLACK = 1,
        SPVC_MSL_SAMPLER_BORDER_COLOR_OPAQUE_WHITE = 2,
        SPVC_MSL_SAMPLER_BORDER_COLOR_INT_MAX = 0x7fffffff,
    }
}
