using System;

namespace Pie;

/// <summary>
/// Used to tell the graphics device how to blend objects with opacity.
/// </summary>
public abstract class BlendState : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this <see cref="BlendState"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Get the <see cref="BlendStateDescription"/> for this <see cref="BlendState"/>.
    /// </summary>
    public abstract BlendStateDescription Description { get; }

    /// <summary>
    /// Dispose of this <see cref="BlendState"/>.
    /// </summary>
    public abstract void Dispose();
}