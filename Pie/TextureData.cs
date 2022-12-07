using System;

namespace Pie;

public unsafe struct TextureData
{
    public void* DataPtr;

    public TextureData(IntPtr data)
    {
        DataPtr = data.ToPointer();
    }

    public TextureData(void* data, uint dataLength)
    {
        DataPtr = data;
    }
    
    public TextureData(byte[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
    }

    public TextureData(int[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
    }

    public TextureData(uint[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
    }

    public TextureData(float[] data)
    {
        fixed (void* dat = data)
            DataPtr = dat;
    }
}