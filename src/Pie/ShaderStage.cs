namespace Pie;

/// <summary>
/// The stage of shader code. These are often combined, for example Vertex and Fragment shaders are often used in one
/// <see cref="Shader"/> object.
/// </summary>
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