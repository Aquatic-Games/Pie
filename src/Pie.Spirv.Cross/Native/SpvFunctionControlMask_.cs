namespace Pie.Spirv.Cross.Native;

public enum SpvFunctionControlMask_
{
    SpvFunctionControlMaskNone = 0,
    SpvFunctionControlInlineMask = 0x00000001,
    SpvFunctionControlDontInlineMask = 0x00000002,
    SpvFunctionControlPureMask = 0x00000004,
    SpvFunctionControlConstMask = 0x00000008,
    SpvFunctionControlOptNoneINTELMask = 0x00010000,
}
