namespace Pie;

public struct TextureDescription
{
    /// <summary>
    /// The type of this texture.
    /// </summary>
    public TextureType TextureType;

    /// <summary>
    /// The width of this texture.
    /// </summary>
    public int Width;

    /// <summary>
    /// The height of this texture.
    /// </summary>
    public int Height;

    /// <summary>
    /// The pixel format of this texture.
    /// </summary>
    public PixelFormat Format;

    /// <summary>
    /// Whether or not mipmaps will be generated.
    /// </summary>
    public bool Mipmap;

    /// <summary>
    /// The number of arrays in this texture. 
    /// </summary>
    public int ArraySize;

    /*/// <summary>
    /// Whether or not this texture is dynamic.
    /// </summary>
    public bool Dynamic;*/

    /// <summary>
    /// The usage of this texture.
    /// </summary>
    public TextureUsage Usage;

    public TextureDescription(TextureType textureType, int width, int height, PixelFormat format, bool mipmap, int arraySize, TextureUsage usage)
    {
        TextureType = textureType;
        Width = width;
        Height = height;
        Format = format;
        Mipmap = mipmap;
        ArraySize = arraySize;
        Usage = usage;
    }
}