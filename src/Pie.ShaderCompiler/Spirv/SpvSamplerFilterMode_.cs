namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvSamplerFilterMode_ : uint
    {
        SpvSamplerFilterModeNearest = 0,
        SpvSamplerFilterModeLinear = 1,
        SpvSamplerFilterModeMax = 0x7fffffff,
    }
}
