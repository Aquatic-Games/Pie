namespace Pie;

public static class PieMetrics
{
    /// <summary>
    /// The total number of vertex buffers currently active in the application.
    /// </summary>
    public static int VertexBufferCount { get; internal set; }
    
    /// <summary>
    /// The total number of index buffers currently active in the application.
    /// </summary>
    public static int IndexBufferCount { get; internal set; }
}