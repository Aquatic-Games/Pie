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
    /// Whether or not mipmaps will be generated.
    /// </summary>
    public bool Mipmap;

    /// <summary>
    /// The number of arrays in this texture. 
    /// </summary>
    public int ArraySize;

    /// <summary>
    /// Whether or not this texture is dynamic.
    /// </summary>
    public bool Dynamic;
    
    
}