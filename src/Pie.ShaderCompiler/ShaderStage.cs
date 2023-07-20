namespace Pie.ShaderCompiler;

/// <summary>
/// Represents various shader stages.
/// </summary>
public enum ShaderStage
{
    /// <summary>
    /// This is a vertex shader.
    /// </summary>
    Vertex,
    
    /// <summary>
    /// This is a fragment shader. (Equivalent to <see cref="Pixel"/>)
    /// </summary>
    Fragment,
    
    /// <summary>
    /// This is a pixel shader. (Equivalent to <see cref="Fragment"/>)
    /// </summary>
    Pixel = Fragment,
    
    /// <summary>
    /// This is a geometry shader.
    /// </summary>
    Geometry,
    
    /// <summary>
    /// This is a compute shader.
    /// </summary>
    Compute
}