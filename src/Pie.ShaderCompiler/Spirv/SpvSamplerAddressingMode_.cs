namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvSamplerAddressingMode_ : uint
    {
        SpvSamplerAddressingModeNone = 0,
        SpvSamplerAddressingModeClampToEdge = 1,
        SpvSamplerAddressingModeClamp = 2,
        SpvSamplerAddressingModeRepeat = 3,
        SpvSamplerAddressingModeRepeatMirrored = 4,
        SpvSamplerAddressingModeMax = 0x7fffffff,
    }
}
