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

    public static unsafe Texture CreateTexture<T>(TextureDescription description, T[] data) where T : unmanaged
    {
        if (description.ArraySize < 1)
            throw new PieException("Array size must be at least 1.");

        int bytesExpected = description.Width * description.Height * 4;
        if (data != null && data.Length != bytesExpected)
            throw new PieException($"{bytesExpected} bytes expected, {data.Length} bytes received.");
        
        uint handle = Gl.GenTexture();
        TextureTarget target = description.TextureType switch
        {
            TextureType.Texture2D => TextureTarget.Texture2D,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Gl.BindTexture(target, handle);

        Silk.NET.OpenGL.PixelFormat fmt = description.Format switch
        {
            PixelFormat.R8G8B8A8_UNorm => Silk.NET.OpenGL.PixelFormat.Rgba,
            PixelFormat.B8G8R8A8_UNorm => Silk.NET.OpenGL.PixelFormat.Bgra,
            _ => throw new ArgumentOutOfRangeException()
        };

        fixed (void* p = data)
        {
            switch (description.TextureType)
            {
                case TextureType.Texture2D:
                    if (description.ArraySize == 1)
                    {
                        Gl.TexImage2D(target, 0, InternalFormat.Rgba8, (uint) description.Width,
                            (uint) description.Height, 0, fmt, PixelType.UnsignedByte, p);
                    }
                    else
                        throw new NotImplementedException("Currently texture arrays have not been implemented.");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (description.Mipmap)
            Gl.GenerateMipmap(TextureTarget.Texture2D);

        return new OpenGL33Texture(handle, fmt, new Size(description.Width, description.Height), description.Mipmap);
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