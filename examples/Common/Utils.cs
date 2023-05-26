using Pie;
using Pie.Audio;

namespace Common;

public static class Utils
{
    public static Texture CreateTexture2D(GraphicsDevice device, string path) => CreateTexture2D(device, new Bitmap(path));
    
    public static Texture CreateTexture2D(GraphicsDevice device, Bitmap bitmap)
    {
        TextureDescription description = TextureDescription.Texture2D(bitmap.Size.Width, bitmap.Size.Height,
            Format.R8G8B8A8_UNorm, 1, 1, TextureUsage.ShaderResource);

        return device.CreateTexture(description, bitmap.Data);
    }

    public static ushort GetFreeChannel(AudioSystem system)
    {
        for (ushort c = 0; c < system.NumChannels; c++)
        {
            if (!system.IsPlaying(c))
                return c;
        }

        return 0;
    }
    
}