using System.Drawing;

namespace Pie;

public abstract class Framebuffer
{
    /// <summary>
    /// Will return <see langword="true" /> when this framebuffer has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The size, in pixels, of this framebuffer.
    /// </summary>
    public abstract Size Size { get; set; }

    public abstract void Dispose();
}