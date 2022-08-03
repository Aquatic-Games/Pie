using System;

namespace Pie;

public abstract class Texture : IDisposable
{
    public abstract void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged;
    
    public abstract void Dispose();
}