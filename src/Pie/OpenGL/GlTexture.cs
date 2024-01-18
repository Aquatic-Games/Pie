using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlTexture : Texture
{
    public int Handle;
    public bool IsRenderbuffer;
    public TextureTarget Target;

    private PixelFormat _fmt;
    private InternalFormat _iFmt;
    private PixelType _pixelType;
    private bool _compressed;

    public GlTexture(int handle, PixelFormat fmt, InternalFormat iFmt, PixelType pixelType, bool compressed, TextureDescription description, bool isRenderbuffer, TextureTarget target)
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
        if (GlGraphicsDevice.IsES && description.TextureType == TextureType.Texture1D)
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
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg8;
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
                iFmt = (InternalFormat) All.Rgba16Snorm;
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
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg32i;
                type = PixelType.Int;
                break;
            case Format.R32G32_UInt:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg32ui;
                type = PixelType.UnsignedInt;
                break;
            case Format.R32G32_Float:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg32f;
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
                iFmt = (InternalFormat) All.Rgb32f;
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
                iFmt = InternalFormat.R8Snorm;
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
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg8Snorm;
                type = PixelType.Byte;
                break;
            case Format.R8G8_SInt:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg8i;
                type = PixelType.Byte;
                break;
            case Format.R8G8_UInt:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg8ui;
                type = PixelType.UnsignedByte;
                break;
            case Format.R8G8B8A8_SNorm:
                fmt = PixelFormat.Rgb;
                iFmt = InternalFormat.Rgba8Snorm;
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
                iFmt = InternalFormat.R16Snorm;
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
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg16;
                type = PixelType.UnsignedShort;
                break;
            case Format.R16G16_SNorm:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg16Snorm;
                type = PixelType.Short;
                break;
            case Format.R16G16_SInt:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg16i;
                type = PixelType.Short;
                break;
            case Format.R16G16_UInt:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg16ui;
                type = PixelType.UnsignedShort;
                break;
            case Format.R16G16_Float:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.Rg16f;
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
                iFmt = InternalFormat.CompressedRgbaS3tcDxt1Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC1_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbS3tcDxt1Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC2_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedRgbaS3tcDxt3Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC2_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC3_UNorm:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedRgbaS3tcDxt5Ext;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC3_UNorm_SRgb:
                fmt = PixelFormat.Rgba;
                iFmt = InternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
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
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.CompressedRgRgtc2;
                type = PixelType.UnsignedByte;
                compressed = true;
                break;
            case Format.BC5_SNorm:
                fmt = PixelFormat.Rg;
                iFmt = InternalFormat.CompressedSignedRgRgtc2;
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
        
        int handle;
        if (isRenderbuffer)
        {
            handle = GL.GenRenderbuffer();
            if (GlGraphicsDevice.Debug)
                PieLog.Log(LogType.Info, "Texture will be created as a Renderbuffer.");
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, handle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage) iFmt, description.Width, description.Height);
        }
        else
        {
            handle = GL.GenTexture();
            
            target = description.TextureType switch
            {
                TextureType.Texture1D => description.ArraySize > 1 ? TextureTarget.Texture1DArray : TextureTarget.Texture1D,
                TextureType.Texture2D => description.ArraySize > 1 ? TextureTarget.Texture2DArray : TextureTarget.Texture2D,
                TextureType.Texture3D => TextureTarget.Texture3D,
                TextureType.Cubemap => description.ArraySize > 1 ? throw new NotImplementedException("Cubemap arrays are not currently supported.") : TextureTarget.TextureCubeMap,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            GL.BindTexture(target, handle);
            PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);

            // Calculate the number of mip levels if the description's value is 0.
            int mipLevels = description.MipLevels == 0
                ? PieUtils.CalculateMipLevels(description.Width, description.Height, description.Depth)
                : description.MipLevels;

            // Allocate the texture based on the target and number of mip levels.
            AllocateTexture(target, mipLevels, (SizedInternalFormat) iFmt, description);

            if (data != null)
            {
                // The current offset in bytes to look at the data.
                int currentOffset = 0;

                for (int a = 0; a < description.ArraySize * (description.TextureType == TextureType.Cubemap ? 6 : 1); a++)
                {
                    int width = description.Width;
                    // While width must always have a width >= 1, height and depth may not always. If the height or depth
                    // are 0, this will cause the size calculation to fail, so we must set it to 1 here.
                    int height = PieUtils.Max(description.Height, 1);
                    int depth = PieUtils.Max(description.Depth, 1);

                    // The loop must run at least once, even if the mip levels are 0.
                    for (int i = 0; i < PieUtils.Max(1, description.MipLevels); i++)
                    {
                        int currSize = (int) (width * height * depth * (bpp / 8f));

                        UpdateSubTexture(target, i, a, 0, 0, 0, width, height, depth, compressed, iFmt, fmt, currSize,
                            type, (IntPtr) ((byte*) data + currentOffset));

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
                GL.TexParameter(target, TextureParameterName.TextureMaxLevel, description.MipLevels - 1);
        }

        return new GlTexture(handle, fmt, iFmt, type, compressed, description, isRenderbuffer, target);
    }

    public override bool IsDisposed { get; protected set; }
    public override TextureDescription Description { get; set; }

    public unsafe void Update(int x, int y, int z, int width, int height, int depth, int mipLevel, int arrayIndex, void* data)
    {
        GL.BindTexture(Target, Handle);

        TextureDescription description = Description;
        PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);
        int size = (int) (PieUtils.Max(description.Width, 1) * PieUtils.Max(description.Height, 1) *
                            PieUtils.Max(description.Depth, 1) * (bpp / 8f));

        UpdateSubTexture(Target, mipLevel, arrayIndex, x, y, z, width, height, depth, _compressed, _iFmt, _fmt, size,
            _pixelType, (IntPtr) data);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        if (IsRenderbuffer)
            GL.DeleteRenderbuffer(Handle);
        else
            GL.DeleteTexture(Handle);
    }

    private static void AllocateTexture(TextureTarget target, int mipLevels, SizedInternalFormat format, in TextureDescription description)
    {
        switch (target)
        {
            case TextureTarget.Texture1D:
                GL.TexStorage1D((TextureTarget1d) target, mipLevels, format, description.Width);
                break;
            case TextureTarget.Texture2D:
                GL.TexStorage2D((TextureTarget2d) target, mipLevels, format, description.Width, description.Height);
                break;
            case TextureTarget.Texture3D:
                GL.TexStorage3D((TextureTarget3d) target, mipLevels, format, description.Width, description.Height,
                    description.Depth);
                break;
            case TextureTarget.Texture1DArray:
                GL.TexStorage2D((TextureTarget2d) target, mipLevels, format, description.Width, description.ArraySize);
                break;
            case TextureTarget.Texture2DArray:
                GL.TexStorage3D((TextureTarget3d) target, mipLevels, format, description.Width, description.Height,
                    description.ArraySize);
                break;
            case TextureTarget.TextureCubeMap:
                GL.TexStorage2D((TextureTarget2d) target, mipLevels, format, description.Width, description.Height);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(target));
        }
    }

    private static void UpdateSubTexture(TextureTarget target, int level, int arrayIndex, int x, int y, int z,
        int width, int height, int depth, bool isCompressed, InternalFormat glIFormat, PixelFormat glFormat,
        int size, PixelType type, IntPtr data)
    {
        if (isCompressed && size < 16)
            return;
        
        switch (target)
        {
            case TextureTarget.Texture1D:
                if (isCompressed)
                    GL.CompressedTexSubImage1D(target, level, x, width, (PixelFormat) glIFormat, size, data);
                else
                    GL.TexSubImage1D(target, level, x, width, glFormat, type, data);
                break;
            case TextureTarget.Texture2D:
                if (isCompressed)
                    GL.CompressedTexSubImage2D(target, level, x, y, width, height, (PixelFormat) glIFormat, size, data);
                else
                    GL.TexSubImage2D(target, level, x, y, width, height, glFormat, type, data);
                break;
            case TextureTarget.Texture3D:
                if (isCompressed)
                    GL.CompressedTexSubImage3D(target, level, x, y, z, width, height, depth, (PixelFormat) glIFormat, size, data);
                else
                    GL.TexSubImage3D(target, level, x, y, z, width, height, depth, glFormat, type, data);
                break;
            case TextureTarget.Texture1DArray:
                if (isCompressed)
                    GL.CompressedTexSubImage2D(target, level, x, arrayIndex, width, 1, (PixelFormat) glIFormat, size, data);
                else
                    GL.TexSubImage2D(target, level, x, arrayIndex, width, 1, glFormat, type, data);
                break;
            case TextureTarget.Texture2DArray:
                if (isCompressed)
                {
                    GL.CompressedTexSubImage3D(target, level, x, y, arrayIndex, width, height, 1, (PixelFormat) glIFormat, size,
                        data);
                }
                else
                    GL.TexSubImage3D(target, level, x, y, arrayIndex, width, height, 1, glFormat, type, data);
                break;
            case TextureTarget.TextureCubeMap:
                TextureTarget cubemapTarget = TextureTarget.TextureCubeMapPositiveX + arrayIndex;

                if (isCompressed)
                    GL.CompressedTexSubImage2D(cubemapTarget, level, x, y, width, height, (PixelFormat) glIFormat, size, data);
                else
                    GL.TexSubImage2D(cubemapTarget, level, x, y, width, height, glFormat, type, data);
                break;
        }
    }
}