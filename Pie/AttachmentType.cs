namespace Pie;

/// <summary>
/// A texture attachment type for a <see cref="Framebuffer"/>.
/// </summary>
public enum AttachmentType
{
    /// <summary>
    /// This attachment is a color attachment.
    /// </summary>
    Color,
    
    /// <summary>
    /// This attachment is a depth/stencil attachment.
    /// </summary>
    DepthStencil
}