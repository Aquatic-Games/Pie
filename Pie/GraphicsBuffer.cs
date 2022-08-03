using System;

namespace Pie;

public abstract class GraphicsBuffer : IDisposable
{
    public abstract bool IsDisposed { get; protected set; }

    public abstract void Update<T>(uint offset, T[] data) where T : unmanaged;
    
    public abstract void Dispose();
}