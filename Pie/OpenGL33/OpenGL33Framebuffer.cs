using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33Framebuffer : Framebuffer
{
    public uint Handle;

    public OpenGL33Framebuffer(FramebufferAttachment[] attachments)
    {
        Handle = Gl.GenFramebuffer();
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

        int colorNum = 0;
        foreach (FramebufferAttachment attachment in attachments)
        {
            Silk.NET.OpenGL.FramebufferAttachment amnt = attachment.AttachmentType switch
            {
                AttachmentType.Color => Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0 + colorNum++,
                AttachmentType.DepthStencil => Silk.NET.OpenGL.FramebufferAttachment.DepthStencilAttachment,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            OpenGL33Texture tex = (OpenGL33Texture) attachment.Texture;
            if (tex.IsRenderbuffer)
            {
                Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, amnt, RenderbufferTarget.Renderbuffer,
                    tex.Handle);
            }
            else
            {
                Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, amnt, TextureTarget.Texture2D, tex.Handle, 0);
            }
        }

        if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
            throw new PieException($"OpenGL: Framebuffer is not complete: {Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
    }
    
    public override bool IsDisposed { get; protected set; }
    public override Size Size { get; set; }
    public override void Dispose()
    {
        Gl.DeleteFramebuffer(Handle);
    }
}