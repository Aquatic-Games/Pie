using System;

namespace Pie;

public abstract class BlendState : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this blend state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public abstract BlendStateDescription Description { get; }

    public abstract void Dispose();
}