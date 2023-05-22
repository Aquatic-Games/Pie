namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvFPDenormMode_ : uint
    {
        SpvFPDenormModePreserve = 0,
        SpvFPDenormModeFlushToZero = 1,
        SpvFPDenormModeMax = 0x7fffffff,
    }
}
