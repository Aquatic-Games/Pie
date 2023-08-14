namespace Pie.ShaderCompiler.Spirv;

internal enum SpvcResult
{
    SpvcSuccess = 0,
    SpvcErrorInvalidSpirv = -1,
    SpvcErrorUnsupportedSpirv = -2,
    SpvcErrorOutOfMemory = -3,
    SpvcErrorInvalidArgument = -4,
    SpvcErrorIntMax = 0x7fffffff,
}