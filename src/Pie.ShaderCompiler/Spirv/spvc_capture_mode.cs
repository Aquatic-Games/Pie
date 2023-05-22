namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum spvc_capture_mode : uint
    {
        SPVC_CAPTURE_MODE_COPY = 0,
        SPVC_CAPTURE_MODE_TAKE_OWNERSHIP = 1,
        SPVC_CAPTURE_MODE_INT_MAX = 0x7fffffff,
    }
}
