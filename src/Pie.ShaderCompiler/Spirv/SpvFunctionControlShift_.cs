namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvFunctionControlShift_ : uint
    {
        SpvFunctionControlInlineShift = 0,
        SpvFunctionControlDontInlineShift = 1,
        SpvFunctionControlPureShift = 2,
        SpvFunctionControlConstShift = 3,
        SpvFunctionControlOptNoneINTELShift = 16,
        SpvFunctionControlMax = 0x7fffffff,
    }
}
