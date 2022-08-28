using System;

namespace Pie;

public abstract class GraphicsBuffer : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this buffer has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    public abstract void Dispose();
}