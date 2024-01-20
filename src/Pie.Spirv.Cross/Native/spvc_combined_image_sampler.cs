namespace Pie.Spirv.Cross.Native;

public partial struct spvc_combined_image_sampler
{
    [NativeTypeName("spvc_variable_id")]
    public uint combined_id;

    [NativeTypeName("spvc_variable_id")]
    public uint image_id;

    [NativeTypeName("spvc_variable_id")]
    public uint sampler_id;
}
