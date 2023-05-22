namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_sampler_ycbcr_range : uint
    {
        SPVC_MSL_SAMPLER_YCBCR_RANGE_ITU_FULL = 0,
        SPVC_MSL_SAMPLER_YCBCR_RANGE_ITU_NARROW,
        SPVC_MSL_SAMPLER_YCBCR_RANGE_INT_MAX = 0x7fffffff,
    }
}
