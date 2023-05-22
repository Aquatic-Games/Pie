namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvRayQueryCandidateIntersectionType_ : uint
    {
        SpvRayQueryCandidateIntersectionTypeRayQueryCandidateIntersectionTriangleKHR = 0,
        SpvRayQueryCandidateIntersectionTypeRayQueryCandidateIntersectionAABBKHR = 1,
        SpvRayQueryCandidateIntersectionTypeMax = 0x7fffffff,
    }
}
