using System;

namespace Pie;

public unsafe struct TextureData
{
    public void* DataPtr;

    public uint DataLength;

    public TextureData(IntPtr data, uint dataLength)
    {
        DataPtr = data.ToPointer();
        DataLength = dataLength;
    }

    public TextureData(void* data, uint dataLength)
    {
        DataPtr = data;
        DataLength = dataLength;
    }
    
    public TextureData(byte[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
        DataLength = (uint) data.Length;
    }

    public TextureData(int[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
        DataLength = (uint) data.Length;
    }

    public TextureData(uint[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
        DataLength = (uint) data.Length;
    }

    public TextureData(float[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
        DataLength = (uint) data.Length;
    }
}