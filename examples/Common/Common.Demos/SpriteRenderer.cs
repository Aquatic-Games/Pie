using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using Common;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Breakout;

// Super simple sprite renderer.
// Does not perform any batching.
// Do note: If you are implementing a sprite renderer for yourself, I recommend using batching.
// However, batching is beyond the scope of the demos as it is not a Pie specific feature.
public class SpriteRenderer : IDisposable
{
    public readonly GraphicsDevice Device;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private ProjModel _projModel;
    private GraphicsBuffer _projModelBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;
    private SamplerState _samplerState;
    private BlendState _blendState;

    public SpriteRenderer(GraphicsDevice device, Size size)
    {
        Device = device;
        
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

        string hlslCode = File.ReadAllText("Shaders/Sprite.hlsl");
        _shader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, hlslCode, Language.HLSL, "VertexShader"),
            new ShaderAttachment(ShaderStage.Pixel, hlslCode, Language.HLSL, "PixelShader")
        }, new []
        {
            // Setting this to 1 will invert the colours.
            // Why? To test specialization constants that's why!
            new SpecializationConstant(0, 0)
        });

        _inputLayout = device.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex)    // texCoord
        );

        _depthStencilState = device.CreateDepthStencilState(DepthStencilStateDescription.Disabled);
        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullClockwise);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.PointClamp);
        _blendState = device.CreateBlendState(BlendStateDescription.NonPremultiplied);

        _projModel = new ProjModel()
        {
            Projection = Matrix4x4.CreateOrthographicOffCenter(0, size.Width, size.Height, 0, -1, 1),
            Model = Matrix4x4.Identity,
            Tint = Vector4.One
        };

        _projModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projModel, true);
    }

    public void Draw(Texture texture, Vector2 position, Color tint, float rotation, Vector2 scale, Vector2 origin)
    {
        Vector2 texSize = new Vector2(texture.Description.Width, texture.Description.Height);
        Matrix4x4 model =
            Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
            Matrix4x4.CreateScale(texSize.X * scale.X, texSize.Y * scale.Y, 1) *
            Matrix4x4.CreateRotationZ(rotation) *
            Matrix4x4.CreateTranslation(position.X, position.Y, 0);
        _projModel.Model = model;
        _projModel.Tint = tint.Normalize();
        
        Device.UpdateBuffer(_projModelBuffer, 0, _projModel);
        Device.SetUniformBuffer(0, _projModelBuffer);
        
        Device.SetPrimitiveType(PrimitiveType.TriangleList);
        Device.SetShader(_shader);
        Device.SetTexture(1, texture, _samplerState);
        Device.SetDepthStencilState(_depthStencilState);
        Device.SetRasterizerState(_rasterizerState);
        Device.SetBlendState(_blendState);
        Device.SetInputLayout(_inputLayout);
        Device.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes);
        Device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        Device.DrawIndexed(6);
    }

    public void Resize(Size size)
    {
        _projModel.Projection = Matrix4x4.CreateOrthographicOffCenter(0, size.Width, size.Height, 0, -1, 1);
    }

    private struct ProjModel
    {
        public Matrix4x4 Projection;
        public Matrix4x4 Model;
        public Vector4 Tint;
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _projModelBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        _depthStencilState.Dispose();
        _rasterizerState.Dispose();
        _samplerState.Dispose();
        _blendState.Dispose();
    }
}