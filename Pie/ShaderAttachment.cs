namespace Pie;

public struct ShaderAttachment
{
    public readonly ShaderStage Stage;
    public readonly string Code;

    internal uint TempHandle;

    public ShaderAttachment(ShaderStage stage, string code)
    {
        Stage = stage;
        Code = code;
        TempHandle = 0;
    }
}