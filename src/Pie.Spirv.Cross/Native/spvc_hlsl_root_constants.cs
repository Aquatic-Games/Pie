namespace Pie.Spirv.Cross.Native;

public partial struct spvc_hlsl_root_constants
{
    [NativeTypeName("unsigned int")]
    public uint start;

    [NativeTypeName("unsigned int")]
    public uint end;

    [NativeTypeName("unsigned int")]
    public uint binding;

    [NativeTypeName("unsigned int")]
    public uint space;
}
