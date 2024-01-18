using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal class GlFramebuffer : Framebuffer
{
    public int Handle;

    // TODO: More options in FramebufferAttachment for both GL and D3D11.
    public unsafe GlFramebuffer(FramebufferAttachment[] attachments)
    {
        Handle = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

        int colorNum = 0;
        List<OpenTK.Graphics.OpenGL4.FramebufferAttachment> colAttachments = new List<OpenTK.Graphics.OpenGL4.FramebufferAttachment>();
        foreach (FramebufferAttachment attachment in attachments)
        {
            OpenTK.Graphics.OpenGL4.FramebufferAttachment glAttachment;
            switch (attachment.Texture.Description.Format)
            {
                case Format.D32_Float:
                case Format.D16_UNorm:
                    glAttachment = OpenTK.Graphics.OpenGL4.FramebufferAttachment.DepthAttachment;
                    break;
                case Format.D24_UNorm_S8_UInt:
                    glAttachment = OpenTK.Graphics.OpenGL4.FramebufferAttachment.DepthStencilAttachment;
                    break;
                default:
                    glAttachment = OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment0 + colorNum;
                    colAttachments.Add(glAttachment);
                    colorNum++;
                    break;
            }
            
            GlTexture tex = (GlTexture) attachment.Texture;
            if (tex.IsRenderbuffer)
            {
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, glAttachment, RenderbufferTarget.Renderbuffer,
                    tex.Handle);
            }
            else
            {
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, glAttachment, TextureTarget.Texture2D, tex.Handle, 0);
            }
        }

        OpenTK.Graphics.OpenGL4.FramebufferAttachment[] drawBuffers = colAttachments.ToArray();
        
        // I really,  r e a l l y  hate this, but it's the most efficient way I can think of to reinterpret cast the array
        // Could maybe use Unsafe.As<>() here, but it didn't feel right.
        fixed (OpenTK.Graphics.OpenGL4.FramebufferAttachment* bufs = drawBuffers)
            GL.DrawBuffers(drawBuffers.Length, (DrawBuffersEnum*) bufs);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            throw new PieException($"OpenGL: Framebuffer is not complete: {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    public override bool IsDisposed { get; protected set; }
    public override Size Size { get; set; }
    public override void Dispose()
    {
        GL.DeleteFramebuffer(Handle);
    }
}