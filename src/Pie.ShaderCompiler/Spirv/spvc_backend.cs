namespace Pie.ShaderCompiler.Spirv;

[NativeTypeName("unsigned int")]
internal enum SpvcBackend : uint
{
    SpvcBackendNone = 0,
    SpvcBackendGlsl = 1,
    SpvcBackendHlsl = 2,
    SpvcBackendMsl = 3,
    SpvcBackendCpp = 4,
    SpvcBackendJson = 5,
    SpvcBackendIntMax = 0x7fffffff,
}