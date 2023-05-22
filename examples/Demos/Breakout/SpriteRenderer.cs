using System;
using System.IO;
using System.Numerics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Breakout;

// A very simple sprite renderer. Just draws sprites as and when you ask it to.
public class SpriteRenderer : IDisposable
{
    private GraphicsDevice _device;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private ProjModel _projModel;
    private GraphicsBuffer _projModelBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;
    private SamplerState _samplerState;

    public SpriteRenderer(GraphicsDevice device)
    {
        _device = device;
        
        VertexPositionTexture[] vertices = new[]
        {
            new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1))
        };

        uint[] indices = new[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, vertices);
        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, indices);

        string hlslCode = File.ReadAllText("Content/Shaders/Sprite.hlsl");
        _shader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, hlslCode, Language.HLSL, "VertexShader"),
            new ShaderAttachment(ShaderStage.Pixel, hlslCode, Language.HLSL, "PixelShader")
        });

        _inputLayout = device.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex)    // texCoord
        );

        _depthStencilState = device.CreateDepthStencilState(DepthStencilStateDescription.Disabled);
        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.PointClamp);

        _projModel = new ProjModel()
        {
            Projection = Matrix4x4.CreateOrthographicOffCenter(0, 800, 600, 0, -1, 1),
            Model = Matrix4x4.Identity
        };

        _projModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projModel, true);
    }

    public void Draw(Texture texture, Vector2 position)
    {
        Matrix4x4 model = Matrix4x4.CreateScale(texture.Description.Width, texture.Description.Height, 1) *
                          Matrix4x4.CreateTranslation(position.X, position.Y, 0);
        _projModel.Model = model;
        
        _device.UpdateBuffer(_projModelBuffer, 0, _projModel);
        _device.SetUniformBuffer(0, _projModelBuffer);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetShader(_shader);
        _device.SetTexture(1, texture, _samplerState);
        _device.SetDepthStencilState(_depthStencilState);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes, _inputLayout);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        _device.DrawIndexed(6);
    }
    
    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
    }

    private struct ProjModel
    {
        public Matrix4x4 Projection;
        public Matrix4x4 Model;
    }
}