using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GlGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GlTexture : Texture
{
    public uint Handle;
    public bool IsRenderbuffer;
    public TextureTarget Target;

    private PixelFormat _fmt;
    private InternalFormat _iFmt;
    private PixelType _pixelType;
    private bool _compressed;

    public unsafe GlTexture(uint handle, PixelFormat fmt, InternalFormat iFmt, PixelType pixelType, bool compressed, TextureDescription description, bool isRenderbuffer, TextureTarget target)
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
        if (IsES && description.TextureType == TextureType.Texture1D)
            throw new NotSupportedException("OpenGL ES does not support 1D textures.");
        
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
                TextureType.Cubemap => description.ArraySize > 1 ? throw new NotImplementedException("Cubemap arrays are not currently supported.") : TextureTarget.TextureCubeMap,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            Gl.BindTexture(target, handle);
            PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);

            // Calculate the number of mip levels if the description's value is 0.
            int mipLevels = description.MipLevels == 0
                ? PieUtils.CalculateMipLevels(description.Width, description.Height, description.Depth)
                : description.MipLevels;

            // Allocate the texture based on the target and number of mip levels.
            AllocateTexture(target, (uint) mipLevels, (SizedInternalFormat) iFmt, description);

            if (data != null)
            {
                // The current offset in bytes to look at the data.
                uint currentOffset = 0;

                for (int a = 0; a < description.ArraySize * (description.TextureType == TextureType.Cubemap ? 6 : 1); a++)
                {
                    uint width = (uint) description.Width;
                    // While width must always have a width >= 1, height and depth may not always. If the height or depth
                    // are 0, this will cause the size calculation to fail, so we must set it to 1 here.
                    uint height = (uint) PieUtils.Max(description.Height, 1);
                    uint depth = (uint) PieUtils.Max(description.Depth, 1);

                    // The loop must run at least once, even if the mip levels are 0.
                    for (int i = 0; i < PieUtils.Max(1, description.MipLevels); i++)
                    {
                        uint currSize = (uint) (width * height * depth * (bpp / 8f));

                        UpdateSubTexture(target, i, a, 0, 0, 0, width, height, depth, compressed, iFmt, fmt, currSize,
                            type, (byte*) data + currentOffset);

                        currentOffset += currSize;

                        // Divide the width and height by 2 for each mip level.
                        width /= 2;
                        height /= 2;
                        depth /= 2;

                        if (width < 1)
                            width = 1;
                        if (height < 1)
                            height = 1;
                        if (depth < 1)
                            depth = 1;
                    }
                }
            }

            if (description.MipLevels != 0)
                Gl.TexParameter(target, TextureParameterName.TextureMaxLevel, description.MipLevels - 1);
        }

        return new GlTexture(handle, fmt, iFmt, type, compressed, description, isRenderbuffer, target);
    }

    public override bool IsDisposed { get; protected set; }
    public override TextureDescription Description { get; set; }

    public unsafe void Update(int x, int y, int z, int width, int height, int depth, int mipLevel, int arrayIndex, void* data)
    {
        Gl.BindTexture(Target, Handle);

        TextureDescription description = Description;
        PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);
        uint size = (uint) (PieUtils.Max(description.Width, 1) * PieUtils.Max(description.Height, 1) *
                            PieUtils.Max(description.Depth, 1) * (bpp / 8f));

        UpdateSubTexture(Target, mipLevel, arrayIndex, x, y, z, (uint) width, (uint) height, (uint) depth,
            _compressed, _iFmt, _fmt, size, _pixelType, data);
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

    private static void AllocateTexture(TextureTarget target, uint mipLevels, SizedInternalFormat format, in TextureDescription description)
    {
        switch (target)
        {
            case TextureTarget.Texture1D:
                Gl.TexStorage1D(target, mipLevels, format, (uint) description.Width);
                break;
            case TextureTarget.Texture2D:
                Gl.TexStorage2D(target, mipLevels, format, (uint) description.Width, (uint) description.Height);
                break;
            case TextureTarget.Texture3D:
                Gl.TexStorage3D(target, mipLevels, format, (uint) description.Width, (uint) description.Height,
                    (uint) description.Depth);
                break;
            case TextureTarget.Texture1DArray:
                Gl.TexStorage2D(target, mipLevels, format, (uint) description.Width, (uint) description.ArraySize);
                break;
            case TextureTarget.Texture2DArray:
                Gl.TexStorage3D(target, mipLevels, format, (uint) description.Width, (uint) description.Height,
                    (uint) description.ArraySize);
                break;
            case TextureTarget.TextureCubeMap:
                Gl.TexStorage2D(target, mipLevels, format, (uint) description.Width, (uint) description.Height);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(target));
        }
    }

    private static unsafe void UpdateSubTexture(TextureTarget target, int level, int arrayIndex, int x, int y, int z,
        uint width, uint height, uint depth, bool isCompressed, InternalFormat glIFormat, PixelFormat glFormat,
        uint size, PixelType type, void* data)
    {
        if (isCompressed && size < 16)
            return;
        
        switch (target)
        {
            case TextureTarget.Texture1D:
                if (isCompressed)
                    Gl.CompressedTexSubImage1D(target, level, x, width, glIFormat, size, data);
                else
                    Gl.TexSubImage1D(target, level, x, width, glFormat, type, data);
                break;
            case TextureTarget.Texture2D:
                if (isCompressed)
                    Gl.CompressedTexSubImage2D(target, level, x, y, width, height, glIFormat, size, data);
                else
                    Gl.TexSubImage2D(target, level, x, y, width, height, glFormat, type, data);
                break;
            case TextureTarget.Texture3D:
                if (isCompressed)
                    Gl.CompressedTexSubImage3D(target, level, x, y, z, width, height, depth, glIFormat, size, data);
                else
                    Gl.TexSubImage3D(target, level, x, y, z, width, height, depth, glFormat, type, data);
                break;
            case TextureTarget.Texture1DArray:
                if (isCompressed)
                    Gl.CompressedTexSubImage2D(target, level, x, arrayIndex, width, 1, glIFormat, size, data);
                else
                    Gl.TexSubImage2D(target, level, x, arrayIndex, width, 1, glFormat, type, data);
                break;
            case TextureTarget.Texture2DArray:
                if (isCompressed)
                {
                    Gl.CompressedTexSubImage3D(target, level, x, y, arrayIndex, width, height, 1, glIFormat, size,
                        data);
                }
                else
                    Gl.TexSubImage3D(target, level, x, y, arrayIndex, width, height, 1, glFormat, type, data);
                break;
            case TextureTarget.TextureCubeMap:
                TextureTarget cubemapTarget = TextureTarget.TextureCubeMapPositiveX + arrayIndex;

                if (isCompressed)
                    Gl.CompressedTexSubImage2D(cubemapTarget, level, x, y, width, height, glIFormat, size, data);
                else
                    Gl.TexSubImage2D(cubemapTarget, level, x, y, width, height, glFormat, type, data);
                break;
        }
    }

    internal override MappedSubresource Map(MapMode mode)
    {
        throw new NotImplementedException();
    }

    internal override void Unmap()
    {
        throw new NotImplementedException();
    }
}