namespace Pie.Spirv.Cross.Native;

public enum SpvMemoryAccessMask_
{
    SpvMemoryAccessMaskNone = 0,
    SpvMemoryAccessVolatileMask = 0x00000001,
    SpvMemoryAccessAlignedMask = 0x00000002,
    SpvMemoryAccessNontemporalMask = 0x00000004,
    SpvMemoryAccessMakePointerAvailableMask = 0x00000008,
    SpvMemoryAccessMakePointerAvailableKHRMask = 0x00000008,
    SpvMemoryAccessMakePointerVisibleMask = 0x00000010,
    SpvMemoryAccessMakePointerVisibleKHRMask = 0x00000010,
    SpvMemoryAccessNonPrivatePointerMask = 0x00000020,
    SpvMemoryAccessNonPrivatePointerKHRMask = 0x00000020,
    SpvMemoryAccessAliasScopeINTELMaskMask = 0x00010000,
    SpvMemoryAccessNoAliasINTELMaskMask = 0x00020000,
}
