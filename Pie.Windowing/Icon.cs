namespace Pie.Windowing;

public struct Icon
{
    public readonly uint Width;
    public readonly uint Height;
    public readonly byte[] Data;

    public Icon(uint width, uint height, byte[] data)
    {
        Width = width;
        Height = height;
        Data = data;
    }
}