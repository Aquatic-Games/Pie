namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvSelectionControlMask_ : uint
    {
        SpvSelectionControlMaskNone = 0,
        SpvSelectionControlFlattenMask = 0x00000001,
        SpvSelectionControlDontFlattenMask = 0x00000002,
    }
}
