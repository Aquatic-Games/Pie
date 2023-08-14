namespace Pie.Windowing;

/// <summary>
/// An RGBA formatted bitmap image with data, width, and a height.
/// </summary>
public struct Icon
{
    /// <summary>
    /// The width of this <see cref="Icon"/>.
    /// </summary>
    public readonly uint Width;
    
    /// <summary>
    /// The height of this <see cref="Icon"/>.
    /// </summary>
    public readonly uint Height;
    
    /// <summary>
    /// The data of this <see cref="Icon"/>.
    /// </summary>
    public readonly byte[] Data;

    /// <summary>
    /// Create a new <see cref="Icon"/>.
    /// </summary>
    /// <param name="width">The width of this <see cref="Icon"/>.</param>
    /// <param name="height">The height of this <see cref="Icon"/>.</param>
    /// <param name="data">The data of this <see cref="Icon"/>.</param>
    public Icon(uint width, uint height, byte[] data)
    {
        Width = width;
        Height = height;
        Data = data;
    }
}