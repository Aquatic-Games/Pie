using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.Graphics.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.Graphics.OpenGL33;

internal class OpenGL33Texture : Texture
{
    public uint Handle;

    private Silk.NET.OpenGL.PixelFormat _format;
    private bool _mipmap;
    
    public unsafe OpenGL33Texture(uint handle, Silk.NET.OpenGL.PixelFormat format, Size size)
    {
        
    }

    public static unsafe Texture CreateTexture<T>(uint width, uint height, PixelFormat format, T[] data, TextureSample sample,
        bool mipmap) where T : unmanaged
    {
        uint handle = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, handle);

        Silk.NET.OpenGL.PixelFormat fmt = format switch
        {
            PixelFormat.RGB8 => Silk.NET.OpenGL.PixelFormat.Rgb,
            PixelFormat.RGBA8 => Silk.NET.OpenGL.PixelFormat.Rgba,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        fixed (void* p = data)
        {
            Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, width, height, 0, fmt,
                PixelType.UnsignedByte, p);
        }

        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
        Gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int) (sample switch
        {
            TextureSample.Linear => mipmap ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear,
            TextureSample.Nearest => mipmap? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest,
            _ => throw new ArgumentOutOfRangeException(nameof(sample), sample, null)
        }));
        Gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int) (sample switch
        {
            TextureSample.Linear => TextureMagFilter.Linear,
            TextureSample.Nearest => TextureMagFilter.Nearest,
            _ => throw new ArgumentOutOfRangeException(nameof(sample), sample, null)
        }));

        return new OpenGL33Texture(handle, fmt, new Size((int) width, (int) height));
    }

    public override bool IsDisposed { get; protected set; }
    public override Size Size { get; set; }

    public override unsafe void Update<T>(int x, int y, uint width, uint height, T[] data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        fixed (void* d = data)
            Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, _format, PixelType.UnsignedByte, d);
        
        if (_mipmap)
            Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Gl.DeleteTexture(Handle);
    }
}