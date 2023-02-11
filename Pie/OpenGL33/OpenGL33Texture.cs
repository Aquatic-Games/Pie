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

    public static unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        PieUtils.CheckIfValid(description);

        bool isRenderbuffer = (description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer &&
                              (description.Usage & TextureUsage.ShaderResource) != TextureUsage.ShaderResource;

        Silk.NET.OpenGL.PixelFormat fmt;
        InternalFormat iFmt;

        switch (description.Format)
        {
            case Format.R8G8B8A8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8;
                break;
            case Format.B8G8R8A8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Bgra;
                iFmt = InternalFormat.Rgba8;
                break;
            case Format.D24_UNorm_S8_UInt:
                fmt = Silk.NET.OpenGL.PixelFormat.DepthStencil;
                iFmt = InternalFormat.Depth24Stencil8;
                break;
            case Format.R8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Red;
                iFmt = InternalFormat.R8;
                break;
            case Format.R8G8_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.RG;
                iFmt = InternalFormat.RG8;
                break;
            case Format.R16G16B16A16_Float:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16f;
                break;
            case Format.R32G32B32A32_Float:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32f;
                break;
            case Format.R16G16B16A16_UNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16;
                break;
            case Format.R16G16B16A16_SNorm:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16SNorm;
                break;
            case Format.R16G16B16A16_SInt:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16i;
                break;
            case Format.R16G16B16A16_UInt:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16ui;
                break;
            case Format.R32G32_SInt:
                fmt = Silk.NET.OpenGL.PixelFormat.RG;
                iFmt = InternalFormat.RG32i;
                break;
            case Format.R32G32_UInt:
                fmt = Silk.NET.OpenGL.PixelFormat.RG;
                iFmt = InternalFormat.RG32ui;
                break;
            case Format.R32G32_Float:
                fmt = Silk.NET.OpenGL.PixelFormat.RG;
                iFmt = InternalFormat.RG32f;
                break;
            case Format.R32G32B32_SInt:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgb;
                iFmt = InternalFormat.Rgb32i;
                break;
            case Format.R32G32B32_UInt:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgb;
                iFmt = InternalFormat.Rgb32ui;
                break;
            case Format.R32G32B32_Float:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgb;
                iFmt = InternalFormat.Rgb32f;
                break;
            case Format.R32G32B32A32_SInt:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32i;
                break;
            case Format.R32G32B32A32_UInt:
                fmt = Silk.NET.OpenGL.PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32ui;
                break;
            case Format.R8_SNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8SNorm;
                break;
            case Format.R8_SInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8i;
                break;
            case Format.R8_UInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8ui;
                break;
            case Format.R8G8_SNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8SNorm;
                break;
            case Format.R8G8_SInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8i;
                break;
            case Format.R8G8_UInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8ui;
                break;
            case Format.R8G8B8A8_SNorm:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.Rgba8SNorm;
                break;
            case Format.R8G8B8A8_SInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8i;
                break;
            case Format.R8G8B8A8_UInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8ui;
                break;
            case Format.R16_UNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16;
                break;
            case Format.R16_SNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16SNorm;
                break;
            case Format.R16_SInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16i;
                break;
            case Format.R16_UInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16ui;
                break;
            case Format.R16_Float:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16f;
                break;
            case Format.R16G16_UNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16;
                break;
            case Format.R16G16_SNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16SNorm;
                break;
            case Format.R16G16_SInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16i;
                break;
            case Format.R16G16_UInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16ui;
                break;
            case Format.R16G16_Float:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16f;
                break;
            case Format.R32_SInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R32i;
                break;
            case Format.R32_UInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R32ui;
                break;
            case Format.R32_Float:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R32f;
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
                TextureType.Texture1D => description.ArraySize > 1 ? TextureTarget.Texture1DArray : TextureTarget.Texture1D,
                TextureType.Texture2D => description.ArraySize > 1 ? TextureTarget.Texture2DArray : TextureTarget.Texture2D,
                TextureType.Texture3D => TextureTarget.Texture3D,
                TextureType.Cubemap => TextureTarget.TextureCubeMap,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            Gl.BindTexture(target, handle);
            switch (description.TextureType)
            {
                case TextureType.Texture1D:
                    if (description.ArraySize == 1)
                    {
                        Gl.TexImage1D(target, 0, iFmt, (uint) description.Width, 0, fmt, PixelType.UnsignedByte, data);
                    }
                    else
                    {
                        Gl.TexImage2D(target, 0, iFmt, (uint) description.Width, (uint) description.ArraySize, 0, fmt,
                            PixelType.UnsignedByte, data);
                    }
                    break;
                case TextureType.Texture2D:
                    if (description.ArraySize == 1)
                    {
                        Gl.TexImage2D(target, 0, iFmt, (uint) description.Width, (uint) description.Height, 0, fmt,
                            PixelType.UnsignedByte, data);
                    }
                    else
                    {
                        Gl.TexImage3D(target, 0, iFmt, (uint) description.Width, (uint) description.Height,
                            (uint) description.ArraySize, 0, fmt, PixelType.UnsignedByte, data);
                    }
                    break;
                case TextureType.Texture3D:
                    if (description.ArraySize > 1)
                        throw new NotSupportedException("3D texture arrays are not supported.");

                    Gl.TexImage3D(target, 0, iFmt, (uint) description.Width, (uint) description.Height,
                        (uint) description.Depth, 0, fmt, PixelType.UnsignedByte, data);
                    break;
                case TextureType.Cubemap:
                    int size = description.Width * description.Height * PieUtils.GetSizeMultiplier(description.Format);
                    
                    for (int i = 0; i < 6; i++)
                    {
                        void* dataPtr = data == null ? null : (byte*) data + i * size;
                        Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, iFmt, (uint) description.Width, (uint) description.Height, 0, fmt, PixelType.UnsignedByte, dataPtr);
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

    public unsafe void Update(int x, int y, uint width, uint height, void* data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, _format, PixelType.UnsignedByte, data);
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