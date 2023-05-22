namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_sampler_mip_filter : uint
    {
        SPVC_MSL_SAMPLER_MIP_FILTER_NONE = 0,
        SPVC_MSL_SAMPLER_MIP_FILTER_NEAREST = 1,
        SPVC_MSL_SAMPLER_MIP_FILTER_LINEAR = 2,
        SPVC_MSL_SAMPLER_MIP_FILTER_INT_MAX = 0x7fffffff,
    }
}
