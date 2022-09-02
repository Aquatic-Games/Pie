using System;

namespace Pie;

public abstract class SamplerState : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this sampler state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    public abstract SamplerStateDescription Description { get; }

    public abstract void Dispose();
}