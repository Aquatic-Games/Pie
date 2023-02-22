namespace Pie;

/// <summary>
/// A texture attachment for a <see cref="Framebuffer"/>
/// </summary>
public struct FramebufferAttachment
{
    /// <summary>
    /// The <see cref="Pie.Texture"/> to attach.
    /// </summary>
    public Texture Texture;

    /// <summary>
    /// Create a new framebuffer attachment.
    /// </summary>
    /// <param name="texture">The <see cref="Pie.Texture"/> to attach.</param>
    public FramebufferAttachment(Texture texture)
    {
        Texture = texture;
    }
}