using System.Drawing;

namespace Pie;

/// <summary>
/// Describes a <see cref="SamplerState"/>.
/// </summary>
public struct SamplerStateDescription
{
    /// <summary>
    /// Use anisotropic filtering (with <see cref="SamplerState.MaxAnisotropicLevels"/> levels), with clamping to edge.
    /// </summary>
    public static readonly SamplerStateDescription AnisotropicClamp =
        new SamplerStateDescription(TextureFilter.Anisotropic, TextureAddress.ClampToEdge, TextureAddress.ClampToEdge,
            TextureAddress.ClampToEdge, SamplerState.MaxAnisotropicLevels, Color.Black, 0, float.MaxValue);

    /// <summary>
    /// Use anisotropic filtering (with <see cref="SamplerState.MaxAnisotropicLevels"/> levels), with repeat.
    /// </summary>
    public static readonly SamplerStateDescription AnisotropicRepeat =
        new SamplerStateDescription(TextureFilter.Anisotropic, TextureAddress.Repeat, TextureAddress.Repeat,
            TextureAddress.Repeat, SamplerState.MaxAnisotropicLevels, Color.Black, 0, float.MaxValue);
    
    /// <summary>
    /// Use linear filtering with clamping to edge.
    /// </summary>
    public static readonly SamplerStateDescription LinearClamp =
        new SamplerStateDescription(TextureFilter.MinMagMipLinear, TextureAddress.ClampToEdge, TextureAddress.ClampToEdge,
            TextureAddress.ClampToEdge, 0, Color.Black, 0, float.MaxValue);
    
    /// <summary>
    /// Use linear filtering with repeat.
    /// </summary>
    public static readonly SamplerStateDescription LinearRepeat =
        new SamplerStateDescription(TextureFilter.MinMagMipLinear, TextureAddress.Repeat, TextureAddress.Repeat,
            TextureAddress.Repeat, 0, Color.Black, 0, float.MaxValue);
    
    /// <summary>
    /// Use point filtering with clamping to edge.
    /// </summary>
    public static readonly SamplerStateDescription PointClamp =
        new SamplerStateDescription(TextureFilter.MinMagMipPoint, TextureAddress.ClampToEdge, TextureAddress.ClampToEdge,
            TextureAddress.ClampToEdge, 0, Color.Black, 0, float.MaxValue);
    
    /// <summary>
    /// Use point filtering with repeat.
    /// </summary>
    public static readonly SamplerStateDescription PointRepeat =
        new SamplerStateDescription(TextureFilter.MinMagMipPoint, TextureAddress.Repeat, TextureAddress.Repeat,
            TextureAddress.Repeat, 0, Color.Black, 0, float.MaxValue);
    
    /// <summary>
    /// The filter to use.
    /// </summary>
    public TextureFilter Filter;

    /// <summary>
    /// The texture address of the U-coordinates (S).
    /// </summary>
    public TextureAddress AddressU;

    /// <summary>
    /// The texture address of the V-coordinates (T).
    /// </summary>
    public TextureAddress AddressV;

    /// <summary>
    /// The texture address of the W-coordinates (R).
    /// </summary>
    public TextureAddress AddressW;

    /// <summary>
    /// The maximum number of anisotropic levels. (Max: <see cref="SamplerState.MaxAnisotropicLevels"/>).
    /// </summary>
    public int MaxAnisotropy;

    /// <summary>
    /// The color of the border, when using <see cref="TextureAddress.ClampToBorder"/>.
    /// </summary>
    public Color BorderColor;

    /// <summary>
    /// The minimum LOD of the <see cref="SamplerState"/>.
    /// </summary>
    public float MinLOD;

    /// <summary>
    /// The maximum LOD of the <see cref="SamplerState"/>.
    /// </summary>
    public float MaxLOD;

    /// <summary>
    /// Create a new <see cref="SamplerStateDescription"/>.
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="addressU">The texture address of the U-coordinates (S).</param>
    /// <param name="addressV">The texture address of the V-coordinates (T).</param>
    /// <param name="addressW">The texture address of the W-coordinates (R).</param>
    /// <param name="maxAnisotropy">The maximum number of anisotropic levels. (Max: <see cref="SamplerState.MaxAnisotropicLevels"/>).</param>
    /// <param name="borderColor">The color of the border, when using <see cref="TextureAddress.ClampToBorder"/>.</param>
    /// <param name="minLod">The minimum LOD of the <see cref="SamplerState"/>.</param>
    /// <param name="maxLod">The maximum LOD of the <see cref="SamplerState"/>.</param>
    public SamplerStateDescription(TextureFilter filter, TextureAddress addressU, TextureAddress addressV, TextureAddress addressW, int maxAnisotropy, Color borderColor, float minLod, float maxLod)
    {
        Filter = filter;
        AddressU = addressU;
        AddressV = addressV;
        AddressW = addressW;
        MaxAnisotropy = maxAnisotropy;
        BorderColor = borderColor;
        MinLOD = minLod;
        MaxLOD = maxLod;
    }
}