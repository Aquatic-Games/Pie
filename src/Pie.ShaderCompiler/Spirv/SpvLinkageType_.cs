namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvLinkageType_ : uint
    {
        SpvLinkageTypeExport = 0,
        SpvLinkageTypeImport = 1,
        SpvLinkageTypeLinkOnceODR = 2,
        SpvLinkageTypeMax = 0x7fffffff,
    }
}
