using System;
using System.Drawing;
using System.Numerics;
using Pie.ShaderCompiler;
using Pie.Tests.Tests.Utils;
using Pie.Utils;

namespace Pie.Tests.Tests;

public class InstancingTest : TestBase
{
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private GraphicsBuffer _matricesBuffer;

    private Camera _camera;
    private Camera.CameraMatrices _cameraMatrices;
    private GraphicsBuffer _cameraBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;
    private SamplerState _samplerState;

    private Texture _texture;
    
    protected override void Initialize()
    {
        base.Initialize();

        VertexPositionTexture[] vertices = new[]
        {
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 0))
        };

        ushort[] indices = new ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        // TODO: Span overloads
        _vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, vertices);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, indices);

        _camera = new Camera(75 * MathF.PI / 180, Window.Size.Width / (float) Window.Size.Height)
        {
            Position = new Vector3(0, 0, 3)
        };
        _cameraMatrices = new Camera.CameraMatrices();
        _cameraBuffer = GraphicsDevice.CreateBuffer(BufferType.UniformBuffer, _cameraMatrices, true);

        _shader = GraphicsDevice.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, ShaderCode, Language.HLSL, "VertexShader"),
            new ShaderAttachment(ShaderStage.Pixel, ShaderCode, Language.HLSL, "PixelShader")
        });

        _inputLayout = GraphicsDevice.CreateInputLayout(new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex)
        });

        _depthStencilState = GraphicsDevice.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);
        _rasterizerState = GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _samplerState = GraphicsDevice.CreateSamplerState(SamplerStateDescription.AnisotropicClamp);

        _texture = Utilities.CreateTexture2D(GraphicsDevice, @"C:\Users\ollie\Pictures\awesomeface.png");
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);

        _cameraMatrices.Projection = _camera.Projection;
        _cameraMatrices.View = _camera.View;
        GraphicsDevice.UpdateBuffer(_cameraBuffer, 0, _cameraMatrices);
        
        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth, 1, 0);
        
        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
        GraphicsDevice.SetShader(_shader);
        
        GraphicsDevice.SetDepthStencilState(_depthStencilState);
        GraphicsDevice.SetRasterizerState(_rasterizerState);
        
        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes, _inputLayout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        
        GraphicsDevice.SetUniformBuffer(0, _cameraBuffer);
        GraphicsDevice.SetTexture(1, _texture, _samplerState);
        
        GraphicsDevice.DrawIndexed(6);
    }

    private const string ShaderCode = @"
struct VSInput
{
    float3 position : POSITION0;
    float2 texCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

struct PSOutput
{
    float4 color : SV_Target0;
};

cbuffer CameraMatrices : register(b0)
{
    float4x4 projection;
    float4x4 view;
}

Texture2D tex : register(t1);
SamplerState state : register(s1);

VSOutput VertexShader(const in VSInput input)
{
    VSOutput output;

    output.position = mul(projection, mul(view, float4(input.position, 1.0)));
    output.texCoord = input.texCoord;

    return output;
}

PSOutput PixelShader(const in VSOutput input)
{
    PSOutput output;

    output.color = tex.Sample(state, input.texCoord);

    return output;
}";
}