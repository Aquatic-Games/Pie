namespace Pie.ShaderCompiler.Spirv;

[NativeTypeName("unsigned int")]
internal enum SpvcCaptureMode : uint
{
    SpvcCaptureModeCopy = 0,
    SpvcCaptureModeTakeOwnership = 1,
    SpvcCaptureModeIntMax = 0x7fffffff,
}