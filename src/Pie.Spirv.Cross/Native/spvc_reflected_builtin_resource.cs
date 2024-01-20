namespace Pie.Spirv.Cross.Native;

public partial struct spvc_reflected_builtin_resource
{
    [NativeTypeName("SpvBuiltIn")]
    public SpvBuiltIn_ builtin;

    [NativeTypeName("spvc_type_id")]
    public uint value_type_id;

    public spvc_reflected_resource resource;
}
