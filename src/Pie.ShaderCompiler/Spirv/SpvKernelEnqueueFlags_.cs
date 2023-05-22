namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvKernelEnqueueFlags_ : uint
    {
        SpvKernelEnqueueFlagsNoWait = 0,
        SpvKernelEnqueueFlagsWaitKernel = 1,
        SpvKernelEnqueueFlagsWaitWorkGroup = 2,
        SpvKernelEnqueueFlagsMax = 0x7fffffff,
    }
}
