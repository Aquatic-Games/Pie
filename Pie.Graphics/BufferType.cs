namespace Pie.Graphics;

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
    ConstantBuffer = UniformBuffer
}