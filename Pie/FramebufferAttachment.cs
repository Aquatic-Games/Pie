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
    /// The <see cref="Pie.AttachmentType"/> of this attachment.
    /// </summary>
    public AttachmentType AttachmentType;

    /// <summary>
    /// Create a new framebuffer attachment.
    /// </summary>
    /// <param name="texture">The <see cref="Pie.Texture"/> to attach.</param>
    /// <param name="attachmentType">The <see cref="Pie.AttachmentType"/> of this attachment.</param>
    public FramebufferAttachment(Texture texture, AttachmentType attachmentType)
    {
        Texture = texture;
        AttachmentType = attachmentType;
    }
}