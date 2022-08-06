using System;

namespace Pie.Graphics;

public abstract class InputLayout : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this input layout has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();
}