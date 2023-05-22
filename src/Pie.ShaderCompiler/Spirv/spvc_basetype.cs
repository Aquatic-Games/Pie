namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_basetype : uint
    {
        SPVC_BASETYPE_UNKNOWN = 0,
        SPVC_BASETYPE_VOID = 1,
        SPVC_BASETYPE_BOOLEAN = 2,
        SPVC_BASETYPE_INT8 = 3,
        SPVC_BASETYPE_UINT8 = 4,
        SPVC_BASETYPE_INT16 = 5,
        SPVC_BASETYPE_UINT16 = 6,
        SPVC_BASETYPE_INT32 = 7,
        SPVC_BASETYPE_UINT32 = 8,
        SPVC_BASETYPE_INT64 = 9,
        SPVC_BASETYPE_UINT64 = 10,
        SPVC_BASETYPE_ATOMIC_COUNTER = 11,
        SPVC_BASETYPE_FP16 = 12,
        SPVC_BASETYPE_FP32 = 13,
        SPVC_BASETYPE_FP64 = 14,
        SPVC_BASETYPE_STRUCT = 15,
        SPVC_BASETYPE_IMAGE = 16,
        SPVC_BASETYPE_SAMPLED_IMAGE = 17,
        SPVC_BASETYPE_SAMPLER = 18,
        SPVC_BASETYPE_ACCELERATION_STRUCTURE = 19,
        SPVC_BASETYPE_INT_MAX = 0x7fffffff,
    }
}
