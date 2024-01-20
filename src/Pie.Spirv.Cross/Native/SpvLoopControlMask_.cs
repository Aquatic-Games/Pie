namespace Pie.Spirv.Cross.Native;

public enum SpvLoopControlMask_
{
    SpvLoopControlMaskNone = 0,
    SpvLoopControlUnrollMask = 0x00000001,
    SpvLoopControlDontUnrollMask = 0x00000002,
    SpvLoopControlDependencyInfiniteMask = 0x00000004,
    SpvLoopControlDependencyLengthMask = 0x00000008,
    SpvLoopControlMinIterationsMask = 0x00000010,
    SpvLoopControlMaxIterationsMask = 0x00000020,
    SpvLoopControlIterationMultipleMask = 0x00000040,
    SpvLoopControlPeelCountMask = 0x00000080,
    SpvLoopControlPartialCountMask = 0x00000100,
    SpvLoopControlInitiationIntervalINTELMask = 0x00010000,
    SpvLoopControlMaxConcurrencyINTELMask = 0x00020000,
    SpvLoopControlDependencyArrayINTELMask = 0x00040000,
    SpvLoopControlPipelineEnableINTELMask = 0x00080000,
    SpvLoopControlLoopCoalesceINTELMask = 0x00100000,
    SpvLoopControlMaxInterleavingINTELMask = 0x00200000,
    SpvLoopControlSpeculatedIterationsINTELMask = 0x00400000,
    SpvLoopControlNoFusionINTELMask = 0x00800000,
}
