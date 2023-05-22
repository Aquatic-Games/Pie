namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvAccessQualifier_ : uint
    {
        SpvAccessQualifierReadOnly = 0,
        SpvAccessQualifierWriteOnly = 1,
        SpvAccessQualifierReadWrite = 2,
        SpvAccessQualifierMax = 0x7fffffff,
    }
}
