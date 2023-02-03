using System;

namespace Pie;

/// <summary>
/// A graphics buffer stores data on the GPU, and can be accessed by the GPU.
/// </summary>
public abstract class GraphicsBuffer : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this buffer has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this <see cref="GraphicsBuffer"/>.
    /// </summary>
    public abstract void Dispose();
}