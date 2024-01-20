namespace Pie.Spirv.Cross.Native;

public enum SpvImageOperandsMask_
{
    SpvImageOperandsMaskNone = 0,
    SpvImageOperandsBiasMask = 0x00000001,
    SpvImageOperandsLodMask = 0x00000002,
    SpvImageOperandsGradMask = 0x00000004,
    SpvImageOperandsConstOffsetMask = 0x00000008,
    SpvImageOperandsOffsetMask = 0x00000010,
    SpvImageOperandsConstOffsetsMask = 0x00000020,
    SpvImageOperandsSampleMask = 0x00000040,
    SpvImageOperandsMinLodMask = 0x00000080,
    SpvImageOperandsMakeTexelAvailableMask = 0x00000100,
    SpvImageOperandsMakeTexelAvailableKHRMask = 0x00000100,
    SpvImageOperandsMakeTexelVisibleMask = 0x00000200,
    SpvImageOperandsMakeTexelVisibleKHRMask = 0x00000200,
    SpvImageOperandsNonPrivateTexelMask = 0x00000400,
    SpvImageOperandsNonPrivateTexelKHRMask = 0x00000400,
    SpvImageOperandsVolatileTexelMask = 0x00000800,
    SpvImageOperandsVolatileTexelKHRMask = 0x00000800,
    SpvImageOperandsSignExtendMask = 0x00001000,
    SpvImageOperandsZeroExtendMask = 0x00002000,
    SpvImageOperandsNontemporalMask = 0x00004000,
    SpvImageOperandsOffsetsMask = 0x00010000,
}
