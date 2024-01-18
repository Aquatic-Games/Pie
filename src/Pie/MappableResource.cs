namespace Pie;

public abstract class MappableResource
{
    internal abstract MappedSubresource Map(MapMode mode);

    internal abstract void Unmap();
}