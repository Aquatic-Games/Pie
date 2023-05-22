namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_sampler_address : uint
    {
        SPVC_MSL_SAMPLER_ADDRESS_CLAMP_TO_ZERO = 0,
        SPVC_MSL_SAMPLER_ADDRESS_CLAMP_TO_EDGE = 1,
        SPVC_MSL_SAMPLER_ADDRESS_CLAMP_TO_BORDER = 2,
        SPVC_MSL_SAMPLER_ADDRESS_REPEAT = 3,
        SPVC_MSL_SAMPLER_ADDRESS_MIRRORED_REPEAT = 4,
        SPVC_MSL_SAMPLER_ADDRESS_INT_MAX = 0x7fffffff,
    }
}
