using System;
using System.Drawing;
using System.Numerics;
using Pie.ShaderCompiler;
using Pie.Tests.Tests.Utils;
using Pie.Utils;
using Pie.Windowing;

namespace Pie.Tests.Tests;

public class InstancingTest : TestBase
{
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private GraphicsBuffer _matricesBuffer;

    private Camera _camera;
    private Vector2 _rotation;
    private Camera.CameraMatrices _cameraMatrices;
    private GraphicsBuffer _cameraBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;
    private SamplerState _samplerState;

    private Texture _texture;

    private const int Width = 100;
    private const int Height = 100;
    private const int Depth = 100;
    
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
        
        int curr = 0;
        Matrix4x4[] matrices = new Matrix4x4[Width * Height * Depth];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    matrices[curr++] = Matrix4x4.CreateTranslation(x, y, z);
                }
            }
        }

        _matricesBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, matrices);

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
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex),
            
            new InputLayoutDescription(Format.R32G32B32A32_Float, 0, 1, InputType.PerInstance),
            new InputLayoutDescription(Format.R32G32B32A32_Float, 16, 1, InputType.PerInstance),
            new InputLayoutDescription(Format.R32G32B32A32_Float, 32, 1, InputType.PerInstance),
            new InputLayoutDescription(Format.R32G32B32A32_Float, 48, 1, InputType.PerInstance)
        });

        _depthStencilState = GraphicsDevice.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);
        _rasterizerState = GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _samplerState = GraphicsDevice.CreateSamplerState(SamplerStateDescription.AnisotropicClamp);

        _texture = Utilities.CreateTexture2D(GraphicsDevice, @"C:\Users\ollie\Pictures\awesomeface.png");
        
        _rotation = Vector2.Zero;
    }

    protected override void Update(double dt)
    {
        base.Update(dt);

        const float speed = 1;
        
        if (IsKeyDown(Key.W))
            _camera.Position += _camera.Forward * speed * (float) dt;
        if (IsKeyDown(Key.S))
            _camera.Position += -_camera.Forward * speed * (float) dt;
        if (IsKeyDown(Key.D))
            _camera.Position += _camera.Right * speed * (float) dt;
        if (IsKeyDown(Key.A))
            _camera.Position += -_camera.Right * speed * (float) dt;
        if (IsKeyDown(Key.Space))
            _camera.Position += _camera.Up * speed * (float) dt;
        if (IsKeyDown(Key.C))
            _camera.Position += -_camera.Up * speed * (float) dt;

        const float mSens = 0.01f;

        _rotation.X -= MouseDelta.X * mSens;
        _rotation.Y -= MouseDelta.Y * mSens;

        _rotation.Y = float.Clamp(_rotation.Y, -MathF.PI / 2, MathF.PI / 2);
        
        _camera.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
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
        GraphicsDevice.SetVertexBuffer(1, _matricesBuffer, 64, _inputLayout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        
        GraphicsDevice.SetUniformBuffer(0, _cameraBuffer);
        GraphicsDevice.SetTexture(1, _texture, _samplerState);
        
        GraphicsDevice.DrawIndexedInstanced(6, Width * Height * Depth);
    }

    private const string ShaderCode = @"
struct VSInput
{
    float3 position : POSITION0;
    float2 texCoord : TEXCOORD0;

    float4 m1 : TEXCOORD1;
    float4 m2 : TEXCOORD2;
    float4 m3 : TEXCOORD3;
    float4 m4 : TEXCOORD4;
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

    float4x4 world = transpose(float4x4(input.m1, input.m2, input.m3, input.m4));

    output.position = mul(projection, mul(view, mul(world, float4(input.position, 1.0))));
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