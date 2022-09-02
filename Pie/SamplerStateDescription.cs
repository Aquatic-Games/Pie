using System.Drawing;

namespace Pie;

public struct SamplerStateDescription
{
    public static readonly SamplerStateDescription AnisotropicClamp =
        new SamplerStateDescription(TextureFilter.Anisotropic, TextureAddress.ClampToEdge, TextureAddress.ClampToEdge,
            TextureAddress.ClampToEdge, 16, Color.Black, 0, float.MaxValue);

    public static readonly SamplerStateDescription AnisotropicRepeat =
        new SamplerStateDescription(TextureFilter.Anisotropic, TextureAddress.Repeat, TextureAddress.Repeat,
            TextureAddress.Repeat, 16, Color.Black, 0, float.MaxValue);
    
    public static readonly SamplerStateDescription LinearClamp =
        new SamplerStateDescription(TextureFilter.MinMagMipLinear, TextureAddress.ClampToEdge, TextureAddress.ClampToEdge,
            TextureAddress.ClampToEdge, 0, Color.Black, 0, float.MaxValue);
    
    public static readonly SamplerStateDescription LinearRepeat =
        new SamplerStateDescription(TextureFilter.MinMagMipLinear, TextureAddress.Repeat, TextureAddress.Repeat,
            TextureAddress.Repeat, 0, Color.Black, 0, float.MaxValue);
    
    public static readonly SamplerStateDescription PointClamp =
        new SamplerStateDescription(TextureFilter.MinMagMipPoint, TextureAddress.ClampToEdge, TextureAddress.ClampToEdge,
            TextureAddress.ClampToEdge, 0, Color.Black, 0, float.MaxValue);
    
    public static readonly SamplerStateDescription PointRepeat =
        new SamplerStateDescription(TextureFilter.MinMagMipPoint, TextureAddress.Repeat, TextureAddress.Repeat,
            TextureAddress.Repeat, 0, Color.Black, 0, float.MaxValue);
    
    public TextureFilter Filter;

    public TextureAddress AddressU;

    public TextureAddress AddressV;

    public TextureAddress AddressW;

    public int MaxAnisotropy;

    public Color BorderColor;

    public float MinLOD;

    public float MaxLOD;

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