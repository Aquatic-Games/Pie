using System.Text;

namespace Pie;

public struct ShaderAttachment
{
    /// <summary>
    /// The stage of this shader attachment.
    /// </summary>
    public ShaderStage Stage;
    
    /// <summary>
    /// The source code of this shader attachment.
    /// </summary>
    public byte[] Source;

    internal uint TempHandle;

    /// <summary>
    /// Create a new shader attachment.
    /// </summary>
    /// <param name="stage">The stage of this shader attachment.</param>
    /// <param name="source">The source code of this shader attachment.</param>
    public ShaderAttachment(ShaderStage stage, byte[] source)
    {
        Stage = stage;
        Source = source;
        TempHandle = 0;
    }
    
    /// <summary>
    /// Create a new shader attachment.
    /// </summary>
    /// <param name="stage">The stage of this shader attachment.</param>
    /// <param name="source">The source code of this shader attachment.</param>
    public ShaderAttachment(ShaderStage stage, string source)
    {
        Stage = stage;
        Source = Encoding.UTF8.GetBytes(source);
        TempHandle = 0;
    }
}