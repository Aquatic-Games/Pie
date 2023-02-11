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
    /// The depth of this texture.
    /// </summary>
    public int Depth;

    /// <summary>
    /// The pixel format of this texture.
    /// </summary>
    public Format Format;

    /// <summary>
    /// The number of mipmaps generated with this texture.
    /// </summary>
    public int MipLevels;

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

    public TextureDescription(TextureType textureType, int width, Format format, int mipLevels,
        int arraySize, TextureUsage usage)
    {
        TextureType = textureType;
        Width = width;
        Height = 0;
        Depth = 0;
        Format = format;
        MipLevels = mipLevels;
        ArraySize = arraySize;
        Usage = usage;
    }
    
    public TextureDescription(TextureType textureType, int width, int height, Format format, int mipLevels,
        int arraySize, TextureUsage usage)
    {
        TextureType = textureType;
        Width = width;
        Height = height;
        Depth = 0;
        Format = format;
        MipLevels = mipLevels;
        ArraySize = arraySize;
        Usage = usage;
    }
    
    public TextureDescription(TextureType textureType, int width, int height, int depth, Format format, int mipLevels,
        int arraySize, TextureUsage usage)
    {
        TextureType = textureType;
        Width = width;
        Height = height;
        Depth = depth;
        Format = format;
        MipLevels = mipLevels;
        ArraySize = arraySize;
        Usage = usage;
    }
}