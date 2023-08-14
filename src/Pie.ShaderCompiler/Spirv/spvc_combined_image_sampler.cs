namespace Pie.ShaderCompiler.Spirv;

internal struct SpvcCombinedImageSampler
{
    [NativeTypeName("spvc_variable_id")]
    public uint CombinedId;

    [NativeTypeName("spvc_variable_id")]
    public uint ImageId;

    [NativeTypeName("spvc_variable_id")]
    public uint SamplerId;
}