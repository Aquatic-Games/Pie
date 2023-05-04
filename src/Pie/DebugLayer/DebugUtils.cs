using System;
using System.Runtime.CompilerServices;

namespace Pie.DebugLayer;

internal static class DebugUtils
{
    #region Internal API

    internal static void CheckIfValid<T>(in TextureDescription description, T[] data)
    {
        if (data == null)
            return;

        int dataLengthInBytes = data.Length * Unsafe.SizeOf<T>();

        PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);

        int expectedDataLength = description.TextureType switch
        {
            TextureType.Texture1D => (int) (description.Width * (bpp / 8f)),
            TextureType.Texture2D => (int) (description.Width * description.Height * (bpp / 8f)),
            TextureType.Texture3D => (int) (description.Width * description.Height * (bpp / 8f)),
            TextureType.Cubemap => (int) (description.Width * description.Height * (bpp / 8f)),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (description.MipLevels > 1)
        {
            int width = description.Width / 2;
            int height = description.Height / 2;

            for (int i = 1; i < description.MipLevels; i++)
            {
                expectedDataLength += (int) (width * height * (bpp / 8f));

                width /= 2;
                height /= 2;

                if (width < 1)
                    width = 1;
                if (height < 1)
                    height = 1;
            }
        }

        int cubemapMultiplier = description.TextureType == TextureType.Cubemap ? 6 : 1;
        expectedDataLength *= description.ArraySize * cubemapMultiplier;

        if (dataLengthInBytes != expectedDataLength)
        {
            PieLog.Log(LogType.Critical,
                $"Invalid data length! Expected {expectedDataLength} bytes, received {dataLengthInBytes}.{(description.ArraySize * cubemapMultiplier > 1 || description.MipLevels > 1 ? $"\nYou must make sure you include ALL mip levels ({description.MipLevels}) for every array texture ({description.ArraySize}) in your initial data.\nThis means your initial data must contain data for a total of {description.MipLevels * description.ArraySize * cubemapMultiplier} textures{(description.TextureType == TextureType.Cubemap ? ", note the extra textures as the texture type is cubemap." : "")}.\nIf you cannot supply the required data at initialization time, set the initial data value to null, and use \"GraphicsDevice.UpdateTexture()\" to update each part of the texture separately." : "")}");
        }
    }

    #endregion
}