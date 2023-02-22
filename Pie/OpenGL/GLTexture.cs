using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GLTexture : Texture
{
    public uint Handle;
    public bool IsRenderbuffer;
    public TextureTarget Target;

    private PixelFormat _fmt;
    private InternalFormat _iFmt;
    private PixelType _pixelType;
    private bool _compressed;

    public unsafe GLTexture(uint handle, PixelFormat fmt, InternalFormat iFmt, PixelType pixelType, bool compressed, TextureDescription description, bool isRenderbuffer, TextureTarget target)
    {
        Handle = handle;
        _fmt = fmt;
        _iFmt = iFmt;
        _pixelType = pixelType;
        _compressed = compressed;
        Description = description;
        IsRenderbuffer = isRenderbuffer;
        Target = target;
        
        // TODO: Move CreateTexture back into the constructor, same for the buffer stuff.
    }

    public static unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        Validity validity = description.Validity;
        if (!validity.IsValid)
            throw new InvalidOperationException(validity.Message);

        bool isRenderbuffer = (description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer &&
                              (description.Usage & TextureUsage.ShaderResource) != TextureUsage.ShaderResource;

        PixelFormat fmt;
        InternalFormat iFmt;
        PixelType type;
        bool compressed = false;

        switch (description.Format)
        {
            case Format.R8G8B8A8_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8;
                type = PixelType.UnsignedByte;
                break;
            case Format.B8G8R8A8_UNorm:
                fmt = PixelFormat.Bgra;
                iFmt = InternalFormat.Rgba8;
                type = PixelType.UnsignedByte;
                break;
            case Format.D24_UNorm_S8_UInt:
                fmt = PixelFormat.DepthStencil;
                iFmt = InternalFormat.Depth24Stencil8;
                type = PixelType.UnsignedByte;
                break;
            case Format.R8_UNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8;
                type = PixelType.UnsignedByte;
                break;
            case Format.R8G8_UNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8;
                type = PixelType.UnsignedByte;
                break;
            case Format.R16G16B16A16_Float:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16f;
                type = PixelType.Float;
                break;
            case Format.R32G32B32A32_Float:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32f;
                type = PixelType.Float;
                break;
            case Format.R16G16B16A16_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16;
                type = PixelType.UnsignedByte;
                break;
            case Format.R16G16B16A16_SNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16SNorm;
                type = PixelType.Byte;
                break;
            case Format.R16G16B16A16_SInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16i;
                type = PixelType.Short;
                break;
            case Format.R16G16B16A16_UInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba16ui;
                type = PixelType.UnsignedByte;
                break;
            case Format.R32G32_SInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG32i;
                type = PixelType.Int;
                break;
            case Format.R32G32_UInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG32ui;
                type = PixelType.UnsignedInt;
                break;
            case Format.R32G32_Float:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG32f;
                type = PixelType.Float;
                break;
            case Format.R32G32B32_SInt:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.Rgb32i;
                type = PixelType.Int;
                break;
            case Format.R32G32B32_UInt:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.Rgb32ui;
                type = PixelType.UnsignedInt;
                break;
            case Format.R32G32B32_Float:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.Rgb32f;
                type = PixelType.Float;
                break;
            case Format.R32G32B32A32_SInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32i;
                type = PixelType.Int;
                break;
            case Format.R32G32B32A32_UInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba32ui;
                type = PixelType.UnsignedInt;
                break;
            case Format.R8_SNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8SNorm;
                type = PixelType.Byte;
                break;
            case Format.R8_SInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8i;
                type = PixelType.Byte;
                break;
            case Format.R8_UInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R8ui;
                type = PixelType.UnsignedByte;
                break;
            case Format.R8G8_SNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8SNorm;
                type = PixelType.Byte;
                break;
            case Format.R8G8_SInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8i;
                type = PixelType.Byte;
                break;
            case Format.R8G8_UInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG8ui;
                type = PixelType.UnsignedByte;
                break;
            case Format.R8G8B8A8_SNorm:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.Rgba8SNorm;
                type = PixelType.Byte;
                break;
            case Format.R8G8B8A8_SInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8i;
                type = PixelType.Byte;
                break;
            case Format.R8G8B8A8_UInt:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Rgba8ui;
                type = PixelType.UnsignedByte;
                break;
            case Format.R16_UNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16;
                type = PixelType.UnsignedShort;
                break;
            case Format.R16_SNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16SNorm;
                type = PixelType.Short;
                break;
            case Format.R16_SInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16i;
                type = PixelType.Short;
                break;
            case Format.R16_UInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16ui;
                type = PixelType.UnsignedShort;
                break;
            case Format.R16_Float:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R16f;
                type = PixelType.Float;
                break;
            case Format.R16G16_UNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16;
                type = PixelType.UnsignedShort;
                break;
            case Format.R16G16_SNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16SNorm;
                type = PixelType.Short;
                break;
            case Format.R16G16_SInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16i;
                type = PixelType.Short;
                break;
            case Format.R16G16_UInt:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16ui;
                type = PixelType.UnsignedShort;
                break;
            case Format.R16G16_Float:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.RG16f;
                type = PixelType.Float;
                break;
            case Format.R32_SInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R32i;
                type = PixelType.Int;
                break;
            case Format.R32_UInt:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R32ui;
                type = PixelType.UnsignedInt;
                break;
            case Format.R32_Float:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.R32f;
                type = PixelType.Float; 
                break;
            case Format.R8G8B8A8_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.Srgb8;
                type = PixelType.UnsignedByte;
                break;
            case Format.B8G8R8A8_UNorm_SRgb:
                fmt = PixelFormat.Bgra;
                iFmt = InternalFormat.Srgb8;
                type = PixelType.UnsignedByte;
                break;
            case Format.D32_Float:
                fmt = PixelFormat.DepthComponent;
                iFmt = InternalFormat.DepthComponent32f;
                type = PixelType.Float;
                break;
            case Format.D16_UNorm:
                fmt = PixelFormat.DepthComponent;
                iFmt = InternalFormat.DepthComponent16;
                type = PixelType.UnsignedShort;
                break;
            case Format.BC1_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedRgbaS3TCDxt1Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC1_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbS3TCDxt1Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC2_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedRgbaS3TCDxt3Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC2_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbAlphaS3TCDxt3Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC3_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedRgbaS3TCDxt5Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC3_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbAlphaS3TCDxt5Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC4_UNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.CompressedRedRgtc1;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC4_SNorm:
                fmt = PixelFormat.Red;
                iFmt = InternalFormat.CompressedSignedRedRgtc1;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC5_UNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.CompressedRGRgtc2;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC5_SNorm:
                fmt = PixelFormat.RG;
                iFmt = InternalFormat.CompressedSignedRGRgtc2;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC6H_UF16:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.CompressedRgbBptcUnsignedFloat;
                type = PixelType.Float;
                compressed = true;
                break;
            case Format.BC6H_SF16:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.CompressedRgbBptcSignedFloat;
                type = PixelType.Float;
                compressed = true;
                break;
            case Format.BC7_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedRgbaBptcUnorm;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC7_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbAlphaBptcUnorm;
                type = PixelType.UnsignedByte;
                compressed = true;
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
                PieLog.Log(LogType.Info, "Texture will be created as a Renderbuffer.");
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
            PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);
            uint size = (uint) (description.Width * description.Height * (bpp / 8f));

            // TODO: Please check to make sure I haven't screwed these up.
            switch (description.TextureType)
            {
                case TextureType.Texture1D:
                    if (description.ArraySize == 1)
                    {
                        if (compressed)
                            Gl.CompressedTexImage1D(target, 0, iFmt, (uint) description.Width, 0, size, data);
                        else
                            Gl.TexImage1D(target, 0, iFmt, (uint) description.Width, 0, fmt, type, data);
                    }
                    else
                    {
                        if (compressed)
                        {
                            Gl.CompressedTexImage2D(target, 0, iFmt, (uint) description.Width,
                                (uint) description.ArraySize, 0, size, data);
                        }
                        else
                        {
                            Gl.TexImage2D(target, 0, iFmt, (uint) description.Width, (uint) description.ArraySize, 0,
                                fmt, type, data);
                        }
                    }
                    break;
                case TextureType.Texture2D:
                    if (description.ArraySize == 1)
                    {
                        if (compressed)
                        {
                            Gl.CompressedTexImage2D(target, 0, iFmt, (uint) description.Width,
                                (uint) description.Height, 0, size, data);
                        }
                        else
                        {
                            Gl.TexImage2D(target, 0, iFmt, (uint) description.Width, (uint) description.Height, 0, fmt,
                                type, data);
                        }
                    }
                    else
                    {
                        if (compressed)
                        {
                            Gl.CompressedTexImage3D(target, 0, iFmt, (uint) description.Width,
                                (uint) description.Height, (uint) description.ArraySize, 0, size, data);
                        }
                        else
                        {
                            Gl.TexImage3D(target, 0, iFmt, (uint) description.Width, (uint) description.Height,
                                (uint) description.ArraySize, 0, fmt, type, data);
                        }
                    }
                    break;
                case TextureType.Texture3D:
                    if (description.ArraySize > 1)
                        throw new NotSupportedException("3D texture arrays are not supported.");

                    Gl.TexImage3D(target, 0, iFmt, (uint) description.Width, (uint) description.Height,
                        (uint) description.Depth, 0, fmt, type, data);
                    break;
                case TextureType.Cubemap:
                    for (int i = 0; i < 6; i++)
                    {
                        void* dataPtr = data == null ? null : (byte*) data + i * size;
                        Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, iFmt, (uint) description.Width,
                            (uint) description.Height, 0, fmt, type, dataPtr);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (description.MipLevels != 0)
                Gl.TexParameter(target, TextureParameterName.TextureMaxLevel, description.MipLevels - 1);
        }

        return new GLTexture(handle, fmt, iFmt, type, compressed, description, isRenderbuffer, target);
    }

    public override bool IsDisposed { get; protected set; }
    public override TextureDescription Description { get; set; }

    public unsafe void Update(int x, int y, int z, int width, int height, int depth, int mipLevel, int arrayIndex, void* data)
    {
        // TODO: PixelType.UnsignedByte here should use texture format.
        Gl.BindTexture(Target, Handle);

        TextureDescription description = Description;
        PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);
        uint size = (uint) (description.Width * description.Height * (bpp / 8f));

        // TODO: Check to make sure I haven't screwed these up as well.
        switch (description.TextureType)
        {
            case TextureType.Texture1D:
                    if (description.ArraySize == 1)
                    {
                        if (_compressed)
                            Gl.CompressedTexSubImage1D(Target, mipLevel, x, (uint) width, _iFmt, size, data);
                        else
                            Gl.TexSubImage1D(Target, mipLevel, x, (uint) width, _fmt, _pixelType, data);
                    }
                    else
                    {
                        if (_compressed)
                        {
                            Gl.CompressedTexSubImage2D(Target, mipLevel, x, 0, (uint) width,
                                (uint) arrayIndex, _iFmt, size, data);
                        }
                        else
                        {
                            Gl.TexSubImage2D(Target, mipLevel, x, 0, (uint) width,
                                (uint) arrayIndex, _fmt, _pixelType, data);
                        }
                    }
                    break;
                case TextureType.Texture2D:
                    if (description.ArraySize == 1)
                    {
                        if (_compressed)
                        {
                            Gl.CompressedTexSubImage2D(Target, mipLevel, x, y, (uint) width,
                                (uint) height, _iFmt, size, data);
                        }
                        else
                        {
                            Gl.TexSubImage2D(Target, mipLevel, x, y, (uint) width,
                                (uint) height, _fmt, _pixelType, data);
                        }
                    }
                    else
                    {
                        if (_compressed)
                        {
                            Gl.CompressedTexSubImage3D(Target, mipLevel, x, y, 0, (uint) width,
                                (uint) height, (uint) arrayIndex, _iFmt, size, data);
                        }
                        else
                        {
                            Gl.TexSubImage3D(Target, mipLevel, x, y, 0, (uint) width,
                                (uint) height, (uint) arrayIndex, _fmt, _pixelType, data);
                        }
                    }
                    break;
                case TextureType.Texture3D:
                    if (description.ArraySize > 1)
                        throw new NotSupportedException("3D texture arrays are not supported.");

                    Gl.TexSubImage3D(Target, mipLevel, x, y, z, (uint) width,
                        (uint) height, (uint) depth, _fmt, _pixelType, data);
                    break;
                case TextureType.Cubemap:
                    Gl.TexSubImage2D(TextureTarget.TextureCubeMapPositiveX + arrayIndex, mipLevel, x, y, (uint) width,
                        (uint) height, _fmt, _pixelType, data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
        }
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