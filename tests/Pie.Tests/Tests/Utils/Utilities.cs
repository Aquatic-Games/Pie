using System.IO;
using StbImageSharp;

namespace Pie.Tests.Tests.Utils;

public static class Utilities
{
    public static Texture CreateTexture2D(GraphicsDevice device, string path)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);

        TextureDescription description = TextureDescription.Texture2D(result.Width, result.Height,
            Format.R8G8B8A8_UNorm, 0, 1, TextureUsage.ShaderResource);

        Texture texture = device.CreateTexture(description, result.Data);
        device.GenerateMipmaps(texture);

        return texture;
    }
}