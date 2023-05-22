namespace Pie.ShaderCompiler.Spirv
{
    public partial struct spvc_msl_shader_interface_var
    {
        [NativeTypeName("unsigned int")]
        public uint location;

        [NativeTypeName("spvc_msl_vertex_format")]
        public spvc_msl_shader_variable_format format;

        [NativeTypeName("SpvBuiltIn")]
        public SpvBuiltIn_ builtin;

        [NativeTypeName("unsigned int")]
        public uint vecsize;
    }
}
