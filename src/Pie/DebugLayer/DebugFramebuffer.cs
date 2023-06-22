using System.Drawing;
using System.Text;
using static Pie.Debugging.DebugGraphicsDevice;

namespace Pie.DebugLayer;

internal sealed class DebugFramebuffer : Framebuffer
{
    public Framebuffer Framebuffer;

    public FramebufferAttachment[] Attachments;
    
    public override bool IsDisposed { get; protected set; }
    
    public override Size Size { get; set; }

    public DebugFramebuffer(FramebufferAttachment[] attachments)
    {
        StringBuilder builder = new StringBuilder();

        FramebufferAttachment[] mainAttachments = new FramebufferAttachment[attachments.Length];

        int numberOfDepthAttachments = 0;
        int index = 0;
        int i = 0;
        foreach (FramebufferAttachment attachment in attachments)
        {
            if (attachment.Texture.IsDisposed)
                PieLog.Log(LogType.Critical, "Attempted to attach a disposed texture!");
            
            switch (attachment.Texture.Description.Format)
            {
                case Format.D32_Float:
                case Format.D16_UNorm:
                case Format.D24_UNorm_S8_UInt:
                    builder.AppendLine($@"    Attachment:
        Type: Depth
        Index: {numberOfDepthAttachments}
        Format: {attachment.Texture.Description.Format}");
                    numberOfDepthAttachments++;
                    break;
                default:
                    builder.AppendLine($@"    Attachment:
        Type: Color
        Index: {index}
        Format: {attachment.Texture.Description.Format}");

                    index++;
                    break;
            }

            mainAttachments[i] = new FramebufferAttachment(((DebugTexture) attachment.Texture).Texture);
            i++;
        }
        
        if (numberOfDepthAttachments > 1)
            PieLog.Log(LogType.Critical, $"Maximum allowable depth attachments is 1, however {numberOfDepthAttachments} were found.");
        
        PieLog.Log(LogType.Debug, $"Framebuffer info:\n{builder}");

        Attachments = attachments;
        
        Framebuffer = Device.CreateFramebuffer(mainAttachments);

        Size = Framebuffer.Size;
    }
    
    public override void Dispose()
    {
        Framebuffer.Dispose();
        IsDisposed = Framebuffer.IsDisposed;
        PieLog.Log(LogType.Debug, "Framebuffer disposed.");
    }
}