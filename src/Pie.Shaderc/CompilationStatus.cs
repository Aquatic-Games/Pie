namespace Pie.Shaderc;

public enum CompilationStatus
{
    Success = 0,
    InvalidStage = 1,
    CompilationError = 2,
    InternalError = 3,
    NullResultObject = 4,
    InvalidAssembly = 5,
    ValidationError = 6,
    TransformationError = 7,
    ConfigurationError = 8,
}
