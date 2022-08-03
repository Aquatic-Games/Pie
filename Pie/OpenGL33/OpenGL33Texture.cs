using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33Texture : Texture
{
    public uint Handle;

    private Silk.NET.OpenGL.PixelFormat _format;
    
    public unsafe OpenGL33Texture(uint width, uint height, PixelFormat format)
    {
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
    }
    
    public override unsafe void Update<T>(int x, int y, uint width, uint height, T[] data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        fixed (void* d = data)
            Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, _format, PixelType.UnsignedByte, d);
    }

    public override void Dispose()
    {
        Gl.DeleteTexture(Handle);
    }
}