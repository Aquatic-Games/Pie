namespace Pie.Graphics;

public struct ShaderAttachment
{
    /// <summary>
    /// The stage of this shader attachment.
    /// </summary>
    public ShaderStage Stage;
    
    /// <summary>
    /// The source code of this shader attachment.
    /// </summary>
    public string Source;

    internal uint TempHandle;

    /// <summary>
    /// Create a new shader attachment.
    /// </summary>
    /// <param name="stage">The stage of this shader attachment.</param>
    /// <param name="source">The source code of this shader attachment.</param>
    public ShaderAttachment(ShaderStage stage, string source)
    {
        Stage = stage;
        Source = source;
        TempHandle = 0;
    }
}