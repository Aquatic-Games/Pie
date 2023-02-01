using System;
using System.IO;
using StbImageSharp;

namespace Pie.Framework;

public class Texture2D : IDisposable
{
    public Texture PieTexture;

    public Texture2D(GraphicsDevice device, string path)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        PieTexture = device.CreateTexture(new TextureDescription(TextureType.Texture2D, result.Width, result.Height,
            Format.R8G8B8A8_UNorm, 0, 1, TextureUsage.ShaderResource), result.Data);
        device.GenerateMipmaps(PieTexture);
    }

    public void Dispose()
    {
        PieTexture.Dispose();
    }
}
