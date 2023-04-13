using System.Drawing;

namespace Pie;

/// <summary>
/// A swapchain contains various buffers that are rendered to. Every <see cref="GraphicsDevice"/> must have a
/// <see cref="Swapchain"/> to render to.
/// </summary>
public class Swapchain
{
    /// <summary>
    /// The size of this <see cref="Swapchain"/>.
    /// </summary>
    public Size Size { get; internal set; }
}