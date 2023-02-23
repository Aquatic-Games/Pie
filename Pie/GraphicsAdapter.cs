namespace Pie;

/// <summary>
/// A physical graphics device in a machine.
/// </summary>
public struct GraphicsAdapter
{
    /// <summary>
    /// The name of this <see cref="GraphicsAdapter"/>.
    /// </summary>
    public readonly string Name;

    public GraphicsAdapter(string name)
    {
        Name = name;
    }
}