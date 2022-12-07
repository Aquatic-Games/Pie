using System;
using System.Drawing;

namespace Pie;

public abstract class Texture : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this texture has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The size, in pixels, of this texture.
    /// </summary>
    public abstract Size Size { get; set; }
    
    public abstract TextureDescription Description { get; set; }

    public abstract void Dispose();
}