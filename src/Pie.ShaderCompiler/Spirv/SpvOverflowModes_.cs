namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvOverflowModes_ : uint
    {
        SpvOverflowModesWRAP = 0,
        SpvOverflowModesSAT = 1,
        SpvOverflowModesSAT_ZERO = 2,
        SpvOverflowModesSAT_SYM = 3,
        SpvOverflowModesMax = 0x7fffffff,
    }
}
