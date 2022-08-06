using System;
using System.Drawing;

namespace Pie.Graphics;

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
    
    /// <summary>
    /// Update a region of this texture with the given data.
    /// </summary>
    /// <param name="x">The x-offset in pixels of the data.</param>
    /// <param name="y">The y-offset in pixels of the data.</param>
    /// <param name="width">The width in pixels of the data.</param>
    /// <param name="height">The height in pixels of the data.</param>
    /// <param name="data">The data itself.</param>
    /// <typeparam name="T">Any unmanaged type, typically <see cref="byte"/> or <see cref="float"/>.</typeparam>
    public abstract void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged;
    
    public abstract void Dispose();
}