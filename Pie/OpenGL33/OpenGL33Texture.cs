using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33Texture : Texture
{
    public uint Handle;
    public bool IsRenderbuffer;
    public TextureTarget Target;

    private Silk.NET.OpenGL.PixelFormat _format;

    public unsafe OpenGL33Texture(uint handle, Silk.NET.OpenGL.PixelFormat format, Size size, TextureDescription description, bool isRenderbuffer, TextureTarget target)
    {
        Handle = handle;
        _format = format;
        Size = size;
        Description = description;
        IsRenderbuffer = isRenderbuffer;
        Target = target;
    }

    public static unsafe Texture CreateTexture(TextureDescription description, TextureData* data)
    {
        PieUtils.CheckIfValid(description);

        bool isRenderbuffer = (description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer &&
                              (description.Usage & TextureUsage.ShaderResource) != TextureUsage.ShaderResource;

        Silk.NET.OpenGL.PixelFormat fmt;
        InternalFormat iFmt;

        switch (description.Format)
        {
            case PixelFormat.R8G8B8A8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8;
                break;
            case PixelFormat.B8G8R8A8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Bgra;
                iFmt = InternalFormat.Rgba8;
                break;
            case PixelFormat.D24_UNorm_S8_UInt:
                fmt = Silk.NET.OpenGL.PixelFormat.DepthStencil;
                iFmt = InternalFormat.Depth24Stencil8;
                break;
            case PixelFormat.R8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Red;
                iFmt = InternalFormat.R8;
                break;
            case PixelFormat.R8G8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.RG;
                iFmt = InternalFormat.RG8;
                break;
            case PixelFormat.R16G16B16A16_Float:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16f;
                break;
            case PixelFormat.R32G32B32A32_Float:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        TextureTarget target = TextureTarget.Texture2D;
        
        uint handle;
        if (isRenderbuffer)
        {
            handle = Gl.GenRenderbuffer();
            if (Debug)
                Logging.Log(LogType.Info, "Texture will be created as a Renderbuffer.");
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handle);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, iFmt, (uint) description.Width, (uint) description.Height);
        }
        else
        {
            handle = Gl.GenTexture();
            
            target = description.TextureType switch
            {
                TextureType.Texture2D => TextureTarget.Texture2D,
                TextureType.Cubemap => TextureTarget.TextureCubeMap,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            Gl.BindTexture(target, handle);
            switch (description.TextureType)
            {
                case TextureType.Texture2D:
                    if (description.ArraySize == 1)
                    {
                        void* dataPtr = data == null ? null : data[0].DataPtr;
                        
                        Gl.TexImage2D(target, 0, iFmt, (uint) description.Width,
                            (uint) description.Height, 0, fmt, PixelType.UnsignedByte, dataPtr);
                    }
                    else
                        throw new NotImplementedException("Currently texture arrays have not been implemented.");
                    break;
                case TextureType.Cubemap:
                    for (int i = 0; i < 6; i++)
                    {
                        void* dataPtr = data == null ? null : data[i].DataPtr;
                        
                        Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, iFmt, (uint) description.Width, (uint) description.Height, 0, fmt,
                            PixelType.UnsignedByte, dataPtr);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (description.MipLevels != 0)
                Gl.TexParameter(target, TextureParameterName.TextureMaxLevel, description.MipLevels - 1);
        }

        return new OpenGL33Texture(handle, fmt, new Size(description.Width, description.Height), description, isRenderbuffer, target);
    }

    public override bool IsDisposed { get; protected set; }
    public override Size Size { get; set; }
    public override TextureDescription Description { get; set; }

    public unsafe void Update(int x, int y, uint width, uint height, TextureData data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, _format, PixelType.UnsignedByte, data.DataPtr);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        if (IsRenderbuffer)
            Gl.DeleteRenderbuffer(Handle);
        else
            Gl.DeleteTexture(Handle);
    }
}