using System;
using System.IO;
using System.Numerics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace RenderDemo;

public class Renderer : IDisposable
{
    private GraphicsDevice _device;
    
    private Shader _shader;
    private InputLayout _layout;

    private GraphicsBuffer _cameraInfoBuffer;
    private GraphicsBuffer _drawInfoBuffer;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;
    private BlendState _blendState;
    private SamplerState _samplerState;

    public Renderer(GraphicsDevice device)
    {
        _device = device;

        string shaderCode = File.ReadAllText("Shaders/Renderer.hlsl");
        _shader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, shaderCode, Language.HLSL, "Vertex"),
            new ShaderAttachment(ShaderStage.Pixel, shaderCode, Language.HLSL, "Pixel")
        });

        _layout = device.CreateInputLayout(new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex) // texCoord
        });

        // sizeof(Matrix4x4) = 64
        // Camera info buffer is just sizeof(CameraInfo) which is just sizeof(Matrix4x4) * 2
        // Draw info buffer is just a single matrix.
        _cameraInfoBuffer = device.CreateBuffer(BufferType.UniformBuffer, 128u, true);
        _drawInfoBuffer = device.CreateBuffer(BufferType.UniformBuffer, 64u, true);

        _depthStencilState = device.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);
        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _blendState = device.CreateBlendState(BlendStateDescription.Disabled);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.AnisotropicClamp);
    }

    public void PrepareForDrawing(in CameraInfo info)
    {
        _device.UpdateBuffer(_cameraInfoBuffer, 0, info);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetShader(_shader);
        _device.SetInputLayout(_layout);
        _device.SetUniformBuffer(0, _cameraInfoBuffer);
        _device.SetUniformBuffer(1, _drawInfoBuffer);
        
        _device.SetDepthStencilState(_depthStencilState);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetBlendState(_blendState);
    }

    public void Draw(Mesh mesh, in Matrix4x4 world)
    {
        _device.UpdateBuffer(_drawInfoBuffer, 0, world);
        
        _device.SetVertexBuffer(0, mesh.VertexBuffer, VertexPositionTextureNormal.SizeInBytes);
        _device.SetIndexBuffer(mesh.IndexBuffer, IndexType.UInt);
        
        _device.DrawIndexed(mesh.NumIndices);
    }
    
    public void Dispose()
    {
        _drawInfoBuffer.Dispose();
        _cameraInfoBuffer.Dispose();
        
        _layout.Dispose();
        _shader.Dispose();
    }

    public struct CameraInfo
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;

        public CameraInfo(Matrix4x4 projection, Matrix4x4 view)
        {
            Projection = projection;
            View = view;
        }
    }
}