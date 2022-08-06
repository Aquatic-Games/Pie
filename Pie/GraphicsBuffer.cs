using System;

namespace Pie;

public abstract class GraphicsBuffer : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this buffer has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Update this buffer with the given data at the given offsetInBytes.
    /// </summary>
    /// <param name="offsetInBytes">The offset in bytes, if any, where the data will be updated.</param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    public abstract void Update<T>(uint offsetInBytes, T[] data) where T : unmanaged;
    
    public abstract void Dispose();
}