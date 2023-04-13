namespace Pie;

/// <summary>
/// Various input primitive types, used for rendering.
/// </summary>
public enum PrimitiveType
{
    /// <summary>
    /// These primitives are a triangle list.
    /// </summary>
    TriangleList,
    
    /// <summary>
    /// These primitives are a triangle strip.
    /// </summary>
    TriangleStrip,
    
    /// <summary>
    /// These primitives are a line list.
    /// </summary>
    LineList,
    
    /// <summary>
    /// These primitives are a line strip.
    /// </summary>
    LineStrip,
    
    /// <summary>
    /// These primitives are a point list.
    /// </summary>
    PointList,
    
    /// <summary>
    /// These primitives are a triangle list, with adjacency.
    /// </summary>
    TriangleListAdjacency,
    
    /// <summary>
    /// These primitives are a triangle strip, with adjacency.
    /// </summary>
    TriangleStripAdjacency,
    
    /// <summary>
    /// These primitives are a line list, with adjacency.
    /// </summary>
    LineListAdjacency,
    
    /// <summary>
    /// These primitives are a line strip, with adjacency.
    /// </summary>
    LineStripAdjacency
}