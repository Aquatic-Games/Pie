namespace Pie.Spirv.Cross.Native;

public enum SpvRayFlagsMask_
{
    SpvRayFlagsMaskNone = 0,
    SpvRayFlagsOpaqueKHRMask = 0x00000001,
    SpvRayFlagsNoOpaqueKHRMask = 0x00000002,
    SpvRayFlagsTerminateOnFirstHitKHRMask = 0x00000004,
    SpvRayFlagsSkipClosestHitShaderKHRMask = 0x00000008,
    SpvRayFlagsCullBackFacingTrianglesKHRMask = 0x00000010,
    SpvRayFlagsCullFrontFacingTrianglesKHRMask = 0x00000020,
    SpvRayFlagsCullOpaqueKHRMask = 0x00000040,
    SpvRayFlagsCullNoOpaqueKHRMask = 0x00000080,
    SpvRayFlagsSkipTrianglesKHRMask = 0x00000100,
    SpvRayFlagsSkipAABBsKHRMask = 0x00000200,
}
