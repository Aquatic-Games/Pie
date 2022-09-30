using System;
using System.Reflection;

namespace Pie;

public static class PieMetrics
{
    public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
    
    /// <summary>
    /// The total number of vertex buffers currently active in the application.
    /// </summary>
    public static ulong VertexBufferCount;

    /// <summary>
    /// The total number of index buffers currently active in the application.
    /// </summary>
    public static ulong IndexBufferCount;

    /// <summary>
    /// The total number of uniform/constant buffers currently active in the application.
    /// </summary>
    public static ulong UniformBufferCount;

    /// <summary>
    /// The total number of draw calls in this frame.
    /// </summary>
    public static ulong DrawCalls;

    /// <summary>
    /// The total number of tris/polygons rendered in this frame.
    /// </summary>
    public static ulong TriCount;
}