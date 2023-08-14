namespace Pie.ShaderCompiler.Spirv;

internal struct SpvcSpecializationConstant
{
    [NativeTypeName("spvc_constant_id")]
    public uint Id;

    [NativeTypeName("unsigned int")]
    public uint ConstantId;
}