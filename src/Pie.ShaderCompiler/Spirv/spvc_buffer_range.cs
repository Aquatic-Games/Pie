namespace Pie.ShaderCompiler.Spirv
{
    public partial struct spvc_buffer_range
    {
        [NativeTypeName("unsigned int")]
        public uint index;

        [NativeTypeName("size_t")]
        public nuint offset;

        [NativeTypeName("size_t")]
        public nuint range;
    }
}
