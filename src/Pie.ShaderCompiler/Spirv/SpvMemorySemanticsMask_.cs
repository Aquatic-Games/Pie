namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvMemorySemanticsMask_ : uint
    {
        SpvMemorySemanticsMaskNone = 0,
        SpvMemorySemanticsAcquireMask = 0x00000002,
        SpvMemorySemanticsReleaseMask = 0x00000004,
        SpvMemorySemanticsAcquireReleaseMask = 0x00000008,
        SpvMemorySemanticsSequentiallyConsistentMask = 0x00000010,
        SpvMemorySemanticsUniformMemoryMask = 0x00000040,
        SpvMemorySemanticsSubgroupMemoryMask = 0x00000080,
        SpvMemorySemanticsWorkgroupMemoryMask = 0x00000100,
        SpvMemorySemanticsCrossWorkgroupMemoryMask = 0x00000200,
        SpvMemorySemanticsAtomicCounterMemoryMask = 0x00000400,
        SpvMemorySemanticsImageMemoryMask = 0x00000800,
        SpvMemorySemanticsOutputMemoryMask = 0x00001000,
        SpvMemorySemanticsOutputMemoryKHRMask = 0x00001000,
        SpvMemorySemanticsMakeAvailableMask = 0x00002000,
        SpvMemorySemanticsMakeAvailableKHRMask = 0x00002000,
        SpvMemorySemanticsMakeVisibleMask = 0x00004000,
        SpvMemorySemanticsMakeVisibleKHRMask = 0x00004000,
        SpvMemorySemanticsVolatileMask = 0x00008000,
    }
}
