namespace Pie.DebugLayer;

/// <summary>
/// The result of a memory leak check.
/// </summary>
public struct MemoryCheckResult
{
    public int NumLeakedItems;

    public MemoryCheckResult(int numLeakedItems)
    {
        NumLeakedItems = numLeakedItems;
    }
}