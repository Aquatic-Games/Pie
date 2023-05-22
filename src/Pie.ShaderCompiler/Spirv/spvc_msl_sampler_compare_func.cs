namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_sampler_compare_func : uint
    {
        SPVC_MSL_SAMPLER_COMPARE_FUNC_NEVER = 0,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_LESS = 1,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_LESS_EQUAL = 2,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_GREATER = 3,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_GREATER_EQUAL = 4,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_EQUAL = 5,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_NOT_EQUAL = 6,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_ALWAYS = 7,
        SPVC_MSL_SAMPLER_COMPARE_FUNC_INT_MAX = 0x7fffffff,
    }
}
