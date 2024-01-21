using System;
using Pie;
using Pie.Utils;

namespace RenderDemo;

public class Mesh : IDisposable
{
    public GraphicsBuffer VertexBuffer;
    public GraphicsBuffer IndexBuffer;

    public uint NumIndices;
    
    public Mesh(GraphicsDevice device, VertexPositionTextureNormal[] vertices, uint[] indices)
    {
        VertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, vertices);
        IndexBuffer = device.CreateBuffer(BufferType.IndexBuffer, indices);

        NumIndices = (uint) indices.Length;
    }
    
    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}