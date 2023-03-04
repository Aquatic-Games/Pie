using Spirzza.Interop.Shaderc;

namespace Pie.ShaderCompiler;

public struct CompilerResult
{
    public readonly byte[] Result;

    public readonly bool IsSuccess;

    public readonly string Error;

    public readonly ReflectionInfo? ReflectionInfo;

    public CompilerResult(byte[] result, bool isSuccess, string error, ReflectionInfo? reflectionInfo)
    {
        Result = result;
        IsSuccess = isSuccess;
        Error = error;
        ReflectionInfo = reflectionInfo;
    }
}