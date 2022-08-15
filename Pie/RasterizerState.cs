using System;

namespace Pie;

public abstract class RasterizerState : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this rasterizer state has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Get or set the face that will be culled on next draw. Set to <see cref="CullFace.None"/> to disable face culling.
    /// </summary>
    public abstract CullFace CullFace { get; }
    
    /// <summary>
    /// Get or set the face cull direction. This will determine which is the front face and which is the back face.
    /// </summary>
    public abstract CullDirection CullDirection { get; }
    
    /// <summary>
    /// Get or set the fill mode that will be used on next draw.
    /// </summary>
    public abstract FillMode FillMode { get; }

    /// <summary>
    /// Enable or disable the scissor test.
    /// </summary>
    public abstract bool EnableScissor { get; }

    public abstract void Dispose();
}