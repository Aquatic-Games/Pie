namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_msl_index_type : uint
    {
        SPVC_MSL_INDEX_TYPE_NONE = 0,
        SPVC_MSL_INDEX_TYPE_UINT16 = 1,
        SPVC_MSL_INDEX_TYPE_UINT32 = 2,
        SPVC_MSL_INDEX_TYPE_MAX_INT = 0x7fffffff,
    }
}
