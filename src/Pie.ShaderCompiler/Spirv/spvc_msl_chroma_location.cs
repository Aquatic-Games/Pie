namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_chroma_location : uint
    {
        SPVC_MSL_CHROMA_LOCATION_COSITED_EVEN = 0,
        SPVC_MSL_CHROMA_LOCATION_MIDPOINT,
        SPVC_MSL_CHROMA_LOCATION_INT_MAX = 0x7fffffff,
    }
}
