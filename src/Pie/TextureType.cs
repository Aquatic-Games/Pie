namespace Pie;

/// <summary>
/// Various supported types of texture creation.
/// </summary>
/// <remarks>You may note items such as Texture2DArray are missing. These are implicitly used when using an ArraySize of >1</remarks>
public enum TextureType
{
    /// <summary>
    /// This texture is a 1D texture (width only).
    /// </summary>
    Texture1D,
    
    /// <summary>
    /// This texture is a 2D texture (width and height).
    /// </summary>
    Texture2D,
    
    /// <summary>
    /// This texture is a 3D texture (width, height, and depth).
    /// </summary>
    Texture3D,

    /// <summary>
    /// This texture is a cubemap (width and height, with an implicitly defined ArraySize of 6 * ArraySize.
    /// </summary>
    Cubemap
}