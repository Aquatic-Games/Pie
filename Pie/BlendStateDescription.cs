namespace Pie;

public struct BlendStateDescription
{
    public static readonly BlendStateDescription NonPremultiplied =
        new BlendStateDescription(BlendType.SrcAlpha, BlendType.OneMinusSrcAlpha);

    public static readonly BlendStateDescription
        AlphaBlend = new BlendStateDescription(BlendType.One, BlendType.OneMinusSrcAlpha);

    public static readonly BlendStateDescription Additive = new BlendStateDescription(BlendType.SrcAlpha, BlendType.One);

    public static readonly BlendStateDescription Opaque = new BlendStateDescription(BlendType.One, BlendType.Zero);
    
    public BlendType Source;
    public BlendType Destination;
    
    public BlendStateDescription(BlendType source, BlendType destination)
    {
        Source = source;
        Destination = destination;
    }
}