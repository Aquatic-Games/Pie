using System;
using System.Collections.Generic;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal class GLFramebuffer : Framebuffer
{
    public uint Handle;

    // TODO: More options in FramebufferAttachment for both GL and D3D11.
    public unsafe GLFramebuffer(FramebufferAttachment[] attachments)
    {
        Handle = Gl.GenFramebuffer();
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

        int colorNum = 0;
        List<GLEnum> colAttachments = new List<GLEnum>();
        foreach (FramebufferAttachment attachment in attachments)
        {
            Silk.NET.OpenGL.FramebufferAttachment glAttachment;
            switch (attachment.Texture.Description.Format)
            {
                case Format.D32_Float:
                case Format.D16_UNorm:
                    glAttachment = Silk.NET.OpenGL.FramebufferAttachment.DepthAttachment;
                    break;
                case Format.D24_UNorm_S8_UInt:
                    glAttachment = Silk.NET.OpenGL.FramebufferAttachment.DepthStencilAttachment;
                    break;
                default:
                    glAttachment = Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0 + colorNum;
                    colAttachments.Add((GLEnum) glAttachment);
                    colorNum++;
                    break;
            }
            
            GLTexture tex = (GLTexture) attachment.Texture;
            if (tex.IsRenderbuffer)
            {
                Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, glAttachment, RenderbufferTarget.Renderbuffer,
                    tex.Handle);
            }
            else
            {
                Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, glAttachment, TextureTarget.Texture2D, tex.Handle, 0);
            }
        }

        GLEnum[] drawBuffers = colAttachments.ToArray();
        fixed (GLEnum* e = drawBuffers)
            Gl.DrawBuffers((uint) drawBuffers.Length, e);

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