namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvLoopControlShift_ : uint
    {
        SpvLoopControlUnrollShift = 0,
        SpvLoopControlDontUnrollShift = 1,
        SpvLoopControlDependencyInfiniteShift = 2,
        SpvLoopControlDependencyLengthShift = 3,
        SpvLoopControlMinIterationsShift = 4,
        SpvLoopControlMaxIterationsShift = 5,
        SpvLoopControlIterationMultipleShift = 6,
        SpvLoopControlPeelCountShift = 7,
        SpvLoopControlPartialCountShift = 8,
        SpvLoopControlInitiationIntervalINTELShift = 16,
        SpvLoopControlMaxConcurrencyINTELShift = 17,
        SpvLoopControlDependencyArrayINTELShift = 18,
        SpvLoopControlPipelineEnableINTELShift = 19,
        SpvLoopControlLoopCoalesceINTELShift = 20,
        SpvLoopControlMaxInterleavingINTELShift = 21,
        SpvLoopControlSpeculatedIterationsINTELShift = 22,
        SpvLoopControlNoFusionINTELShift = 23,
        SpvLoopControlMax = 0x7fffffff,
    }
}
