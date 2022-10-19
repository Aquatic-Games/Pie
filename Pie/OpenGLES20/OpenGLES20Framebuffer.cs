using System;
using System.Collections.Generic;
using System.Drawing;
using Silk.NET.OpenGLES;
using static Pie.OpenGLES20.OpenGLES20GraphicsDevice;

namespace Pie.OpenGLES20;

internal class OpenGLES20Framebuffer : Framebuffer
{
    public uint Handle;

    public readonly GLEnum[] DrawBuffers;

    public OpenGLES20Framebuffer(FramebufferAttachment[] attachments)
    {
        Handle = Gl.GenFramebuffer();
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

        int colorNum = 0;
        List<GLEnum> colAttachments = new List<GLEnum>();
        foreach (FramebufferAttachment attachment in attachments)
        {
            Silk.NET.OpenGLES.FramebufferAttachment amnt;
            switch (attachment.AttachmentType)
            {
                case AttachmentType.Color:
                    amnt = Silk.NET.OpenGLES.FramebufferAttachment.ColorAttachment0 + colorNum;
                    colAttachments.Add(GLEnum.ColorAttachment0 + colorNum++);
                    break;
                case AttachmentType.DepthStencil:
                    amnt = Silk.NET.OpenGLES.FramebufferAttachment.DepthStencilAttachment;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OpenGLES20Texture tex = (OpenGLES20Texture) attachment.Texture;
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

        DrawBuffers = colAttachments.ToArray();

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