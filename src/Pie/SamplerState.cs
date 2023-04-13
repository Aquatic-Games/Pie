using System;

namespace Pie;

/// <summary>
/// Used to tell the graphics device how to sample textures.
/// </summary>
public abstract class SamplerState : IDisposable
{
    /// <summary>
    /// The maximum number of anisotropic levels a texture can have.
    /// </summary>
    public const int MaxAnisotropicLevels = 16;
    
    /// <summary>
    /// Will return <see langword="true" /> when this <see cref="SamplerState"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The <see cref="SamplerStateDescription"/> of this <see cref="SamplerState"/>.
    /// </summary>
    public abstract SamplerStateDescription Description { get; }

    /// <summary>
    /// Dispose of this <see cref="SamplerState"/>.
    /// </summary>
    public abstract void Dispose();
}