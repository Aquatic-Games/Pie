namespace Pie.Spirv.Cross.Native;

public partial struct spvc_hlsl_resource_binding_mapping
{
    [NativeTypeName("unsigned int")]
    public uint register_space;

    [NativeTypeName("unsigned int")]
    public uint register_binding;
}
