using System.Drawing;

namespace Pie;

public abstract class Framebuffer
{
    /// <summary>
    /// Will return <see langword="true" /> when this <see cref="Framebuffer"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The size, in pixels, of this <see cref="Framebuffer"/>.
    /// </summary>
    public abstract Size Size { get; set; }

    /// <summary>
    /// Dispose of this <see cref="Framebuffer"/>
    /// </summary>
    public abstract void Dispose();
}