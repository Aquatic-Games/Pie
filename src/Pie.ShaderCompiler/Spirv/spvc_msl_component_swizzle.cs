namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_component_swizzle : uint
    {
        SPVC_MSL_COMPONENT_SWIZZLE_IDENTITY = 0,
        SPVC_MSL_COMPONENT_SWIZZLE_ZERO,
        SPVC_MSL_COMPONENT_SWIZZLE_ONE,
        SPVC_MSL_COMPONENT_SWIZZLE_R,
        SPVC_MSL_COMPONENT_SWIZZLE_G,
        SPVC_MSL_COMPONENT_SWIZZLE_B,
        SPVC_MSL_COMPONENT_SWIZZLE_A,
        SPVC_MSL_COMPONENT_SWIZZLE_INT_MAX = 0x7fffffff,
    }
}
