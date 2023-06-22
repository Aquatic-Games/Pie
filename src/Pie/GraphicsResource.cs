namespace Pie;

public abstract class GraphicsResource
{
    internal abstract MappedSubresource Map(MapMode mode);

    internal abstract void Unmap();
}