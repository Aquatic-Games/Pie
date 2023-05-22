namespace Pie.ShaderCompiler.Spirv
{
    public unsafe partial struct spvc_reflected_resource
    {
        [NativeTypeName("spvc_variable_id")]
        public uint id;

        [NativeTypeName("spvc_type_id")]
        public uint base_type_id;

        [NativeTypeName("spvc_type_id")]
        public uint type_id;

        [NativeTypeName("const char *")]
        public sbyte* name;
    }
}
