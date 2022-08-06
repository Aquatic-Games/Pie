using System;

namespace Pie;

public abstract class InputLayout : IDisposable
{
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();
}