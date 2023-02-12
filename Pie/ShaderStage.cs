namespace Pie;

public enum ShaderStage
{
    /// <summary>
    /// This shader is a vertex shader.
    /// </summary>
    Vertex,
    
    /// <summary>
    /// This shader is a fragment shader. (Equivalent to <see cref="Pixel"/>.)
    /// </summary>
    Fragment,
    
    /// <summary>
    /// This shader is a pixel shader. (Equivalent to <see cref="Fragment"/>.)
    /// </summary>
    Pixel = Fragment,
    
    /// <summary>
    /// This shader is a geometry shader.
    /// </summary>
    Geometry,
    
    /// <summary>
    /// This shader is a compute shader.
    /// </summary>
    Compute
}