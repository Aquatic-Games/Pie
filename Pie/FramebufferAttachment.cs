namespace Pie;

public struct FramebufferAttachment
{
    public Texture Texture;

    public AttachmentType AttachmentType;

    public FramebufferAttachment(Texture texture, AttachmentType attachmentType)
    {
        Texture = texture;
        AttachmentType = attachmentType;
    }
}