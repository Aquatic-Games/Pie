using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33Texture : Texture
{
    public uint Handle;

    private Silk.NET.OpenGL.PixelFormat _format;
    private bool _mipmap;
    
    public unsafe OpenGL33Texture(uint width, uint height, PixelFormat format, TextureSample sample, bool mipmap)
    {
        _mipmap = mipmap;
        
        Handle = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, Handle);

        _format = format switch
        {
            PixelFormat.RGB8 => Silk.NET.OpenGL.PixelFormat.Rgb,
            PixelFormat.RGBA8 => Silk.NET.OpenGL.PixelFormat.Rgba,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, width, height, 0, _format,
            PixelType.UnsignedByte, null);
        
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

        Size = new Size((int) width, (int) height);
    }

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
        Gl.DeleteTexture(Handle);
    }
}