namespace Pie.ShaderCompiler.Spirv;

[NativeTypeName("unsigned int")]
internal enum SpvExecutionModel : uint
{
    SpvExecutionModelVertex = 0,
    SpvExecutionModelTessellationControl = 1,
    SpvExecutionModelTessellationEvaluation = 2,
    SpvExecutionModelGeometry = 3,
    SpvExecutionModelFragment = 4,
    SpvExecutionModelGlCompute = 5,
    SpvExecutionModelKernel = 6,
    SpvExecutionModelTaskNv = 5267,
    SpvExecutionModelMeshNv = 5268,
    SpvExecutionModelRayGenerationKhr = 5313,
    SpvExecutionModelRayGenerationNv = 5313,
    SpvExecutionModelIntersectionKhr = 5314,
    SpvExecutionModelIntersectionNv = 5314,
    SpvExecutionModelAnyHitKhr = 5315,
    SpvExecutionModelAnyHitNv = 5315,
    SpvExecutionModelClosestHitKhr = 5316,
    SpvExecutionModelClosestHitNv = 5316,
    SpvExecutionModelMissKhr = 5317,
    SpvExecutionModelMissNv = 5317,
    SpvExecutionModelCallableKhr = 5318,
    SpvExecutionModelCallableNv = 5318,
    SpvExecutionModelTaskExt = 5364,
    SpvExecutionModelMeshExt = 5365,
    SpvExecutionModelMax = 0x7fffffff,
}