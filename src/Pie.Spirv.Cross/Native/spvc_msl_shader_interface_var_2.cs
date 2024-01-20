namespace Pie.Spirv.Cross.Native;

public partial struct spvc_msl_shader_interface_var_2
{
    [NativeTypeName("unsigned int")]
    public uint location;

    public spvc_msl_shader_variable_format format;

    [NativeTypeName("SpvBuiltIn")]
    public SpvBuiltIn_ builtin;

    [NativeTypeName("unsigned int")]
    public uint vecsize;

    public spvc_msl_shader_variable_rate rate;
}
