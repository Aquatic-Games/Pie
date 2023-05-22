namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvScope_ : uint
    {
        SpvScopeCrossDevice = 0,
        SpvScopeDevice = 1,
        SpvScopeWorkgroup = 2,
        SpvScopeSubgroup = 3,
        SpvScopeInvocation = 4,
        SpvScopeQueueFamily = 5,
        SpvScopeQueueFamilyKHR = 5,
        SpvScopeShaderCallKHR = 6,
        SpvScopeMax = 0x7fffffff,
    }
}
