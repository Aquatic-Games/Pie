namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvFPFastMathModeMask_ : uint
    {
        SpvFPFastMathModeMaskNone = 0,
        SpvFPFastMathModeNotNaNMask = 0x00000001,
        SpvFPFastMathModeNotInfMask = 0x00000002,
        SpvFPFastMathModeNSZMask = 0x00000004,
        SpvFPFastMathModeAllowRecipMask = 0x00000008,
        SpvFPFastMathModeFastMask = 0x00000010,
        SpvFPFastMathModeAllowContractFastINTELMask = 0x00010000,
        SpvFPFastMathModeAllowReassocINTELMask = 0x00020000,
    }
}
