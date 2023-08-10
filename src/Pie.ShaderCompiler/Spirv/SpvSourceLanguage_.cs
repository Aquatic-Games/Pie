namespace Pie.ShaderCompiler.Spirv;

[NativeTypeName("unsigned int")]
internal enum SpvSourceLanguage : uint
{
    SpvSourceLanguageUnknown = 0,
    SpvSourceLanguageEssl = 1,
    SpvSourceLanguageGlsl = 2,
    SpvSourceLanguageOpenClC = 3,
    SpvSourceLanguageOpenClCpp = 4,
    SpvSourceLanguageHlsl = 5,
    SpvSourceLanguageCppForOpenCl = 6,
    SpvSourceLanguageSycl = 7,
    SpvSourceLanguageMax = 0x7fffffff,
}