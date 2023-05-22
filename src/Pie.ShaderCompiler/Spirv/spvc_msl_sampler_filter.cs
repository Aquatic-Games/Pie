namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_sampler_filter : uint
    {
        SPVC_MSL_SAMPLER_FILTER_NEAREST = 0,
        SPVC_MSL_SAMPLER_FILTER_LINEAR = 1,
        SPVC_MSL_SAMPLER_FILTER_INT_MAX = 0x7fffffff,
    }
}
