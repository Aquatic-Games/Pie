using System.Drawing;
using System.IO;
using StbImageSharp;

namespace Common;

public class Bitmap
{
    public readonly byte[] Data;
    public readonly Size Size;

    public Bitmap(string path)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);

        Data = result.Data;
        Size = new Size(result.Width, result.Height);
    }
}