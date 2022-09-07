using System;
using System.Drawing;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33Texture : Texture
{
    public uint Handle;

    private Silk.NET.OpenGL.PixelFormat _format;
    private bool _mipmap;
    
    public unsafe OpenGL33Texture(uint handle, Silk.NET.OpenGL.PixelFormat format, Size size, bool mipmap)
    {
        Handle = handle;
        _format = format;
        Size = size;
        _mipmap = mipmap;
    }

    public static unsafe Texture CreateTexture<T>(int width, int height, PixelFormat format, T[] data, bool mipmap) where T : unmanaged
    {
        uint handle = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, handle);

        Silk.NET.OpenGL.PixelFormat fmt = format switch
        {
            PixelFormat.R8G8B8A8_UNorm => Silk.NET.OpenGL.PixelFormat.Rgba,
            PixelFormat.B8G8R8A8_UNorm => Silk.NET.OpenGL.PixelFormat.Bgra,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        fixed (void* p = data)
        {
            Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint) width, (uint) height, 0, fmt,
                PixelType.UnsignedByte, p);
        }

        if (mipmap)
            Gl.GenerateMipmap(TextureTarget.Texture2D);

        return new OpenGL33Texture(handle, fmt, new Size(width, height), mipmap);
    }

    public override bool IsDisposed { get; protected set; }
    public override Size Size { get; set; }

    public unsafe void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged
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