namespace Pie.Spirv.Cross.Native;

public unsafe partial struct spvc_hlsl_vertex_attribute_remap
{
    [NativeTypeName("unsigned int")]
    public uint location;

    [NativeTypeName("const char *")]
    public sbyte* semantic;
}
