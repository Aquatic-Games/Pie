using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pie;

public unsafe struct TextureData
{
    public byte[] Data;

    public TextureData(byte[] data)
    {
        Data = data;
    }

    public TextureData(float[] data)
    {
        Data = Unsafe.As<float[], byte[]>(ref data);
    }
}