namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvFPFastMathModeShift_ : uint
    {
        SpvFPFastMathModeNotNaNShift = 0,
        SpvFPFastMathModeNotInfShift = 1,
        SpvFPFastMathModeNSZShift = 2,
        SpvFPFastMathModeAllowRecipShift = 3,
        SpvFPFastMathModeFastShift = 4,
        SpvFPFastMathModeAllowContractFastINTELShift = 16,
        SpvFPFastMathModeAllowReassocINTELShift = 17,
        SpvFPFastMathModeMax = 0x7fffffff,
    }
}
