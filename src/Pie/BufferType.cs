namespace Pie;

/// <summary>
/// The type of a <see cref="GraphicsBuffer"/>.
/// </summary>
public enum BufferType
{
    /// <summary>
    /// This buffer should be a vertex buffer.
    /// </summary>
    VertexBuffer,
    
    /// <summary>
    /// This buffer should be an index buffer.
    /// </summary>
    IndexBuffer,
    
    /// <summary>
    /// This buffer should be a uniform buffer. (Equivalent to <see cref="ConstantBuffer"/>)
    /// </summary>
    UniformBuffer,
    
    /// <summary>
    /// This buffer should be a constant buffer. (Equivalent to <see cref="UniformBuffer"/>)
    /// </summary>
    ConstantBuffer = UniformBuffer,
    
    /// <summary>
    /// This buffer should be a shader storage buffer. (Equivalent to <see cref="StructuredBuffer"/>)
    /// </summary>
    ShaderStorageBuffer,
    
    /// <summary>
    /// This buffer should be a structured buffer. (Equivalent to <see cref="ShaderStorageBuffer"/>)
    /// </summary>
    StructuredBuffer = ShaderStorageBuffer
}