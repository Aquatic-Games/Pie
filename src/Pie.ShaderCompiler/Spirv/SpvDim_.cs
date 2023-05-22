namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvDim_ : uint
    {
        SpvDim1D = 0,
        SpvDim2D = 1,
        SpvDim3D = 2,
        SpvDimCube = 3,
        SpvDimRect = 4,
        SpvDimBuffer = 5,
        SpvDimSubpassData = 6,
        SpvDimMax = 0x7fffffff,
    }
}
