using Spirzza.Interop.Shaderc;

namespace Pie.ShaderCompiler;

public struct CompilerResult
{
    public readonly string Result;

    public readonly bool Success;

    public readonly string Error;

    public ReflectionInfo? ReflectionInfo;

    public ShaderStage Stage;

    public CompilerResult(string result, bool success, string error, ReflectionInfo? reflectionInfo, ShaderStage stage)
    {
        Result = result;
        Success = success;
        Error = error;
        ReflectionInfo = reflectionInfo;
        Stage = stage;
    }
}