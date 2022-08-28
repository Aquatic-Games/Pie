using System;

namespace Pie;

public abstract class DepthState : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this depth state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public abstract DepthStateDescription Description { get; }

    public abstract void Dispose();
}