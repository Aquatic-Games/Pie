namespace Pie.ShaderCompiler;

/// <summary>
/// Represents various shader stages.
/// </summary>
public enum Stage
{
    /// <summary>
    /// This is a vertex shader.
    /// </summary>
    Vertex,
    
    /// <summary>
    /// This is a fragment shader.
    /// </summary>
    Fragment,
    
    /// <summary>
    /// This is a geometry shader.
    /// </summary>
    Geometry,
    
    /// <summary>
    /// This is a compute shader.
    /// </summary>
    Compute
}