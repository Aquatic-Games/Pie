namespace Pie.ShaderCompiler.Spirv
{
    public unsafe partial struct spvc_entry_point
    {
        [NativeTypeName("SpvExecutionModel")]
        public SpvExecutionModel_ execution_model;

        [NativeTypeName("const char *")]
        public sbyte* name;
    }
}
