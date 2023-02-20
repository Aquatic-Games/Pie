using System;
using System.Drawing;

namespace Pie;

/// <summary>
/// A texture is a data store that can be sampled from in a <see cref="Shader"/>. This includes preloaded texture data,
/// or a <see cref="Framebuffer"/>.
/// </summary>
public abstract class Texture : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this <see cref="Texture"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// The <see cref="TextureDescription"/> of this <see cref="Texture"/>.
    /// </summary>
    public abstract TextureDescription Description { get; set; }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public abstract void Dispose();
}