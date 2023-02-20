using System;
using System.Numerics;

namespace Pie;

/// <summary>
/// A shader is a small program that is executed on the GPU. They are often used to transform vertices and draw pixels
/// (fragments) to the screen.
/// </summary>
public abstract class Shader : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this <see cref="Shader"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The <see cref="Pie.ReflectionInfo"/> of this <see cref="Shader"/>, if any.
    /// </summary>
    public ReflectionInfo[] ReflectionInfo { get; set; }

    /// <summary>
    /// Dispose of this <see cref="Shader"/>.
    /// </summary>
    public abstract void Dispose();
}