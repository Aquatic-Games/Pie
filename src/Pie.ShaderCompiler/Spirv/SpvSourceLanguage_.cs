namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvSourceLanguage_ : uint
    {
        SpvSourceLanguageUnknown = 0,
        SpvSourceLanguageESSL = 1,
        SpvSourceLanguageGLSL = 2,
        SpvSourceLanguageOpenCL_C = 3,
        SpvSourceLanguageOpenCL_CPP = 4,
        SpvSourceLanguageHLSL = 5,
        SpvSourceLanguageCPP_for_OpenCL = 6,
        SpvSourceLanguageSYCL = 7,
        SpvSourceLanguageMax = 0x7fffffff,
    }
}
