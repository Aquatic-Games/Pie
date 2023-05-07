using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using Pie.ShaderCompiler;
using Pie.Tests.Tests.Utils;
using Pie.Utils;
using StbImageSharp;

namespace Pie.Tests.Tests;

// Basic texture test to make sure textures work as expected.
public class TextureTest : TestBase
{
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private Texture _texture;
    private SamplerState _samplerState;

    private Shader _shader;
    private InputLayout _layout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;
    
    protected override void Initialize()
    {
        base.Initialize();

        VertexPositionTexture[] vertices = new[]
        {
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, 0.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, 0.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 0.0f), new Vector2(0, 1)),
        };

        uint[] indices = new[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        const string shader = @"
struct VSInput
{
    float3 position: POSITION;
    float2 texCoords: TEXCOORD0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float2 texCoords: TEXCOORD0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

Texture2D tex : register(t0);
SamplerState samp : register(s0);

VSOutput VertexShader(in VSInput input)
{
    VSOutput output;
    output.position = float4(input.position, 1.0);
    output.texCoords = input.texCoords;
    return output;
}

PSOutput PixelShader(in VSOutput input)
{
    PSOutput output;
    output.color = tex.Sample(samp, input.texCoords);
    return output;
}";

        _vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, vertices);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, indices);
        
        ImageResult result1 = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/awesomeface.png"), ColorComponents.RedGreenBlueAlpha);
        /*ImageResult result2 = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/piegfx-logo-square-temp.png"), ColorComponents.RedGreenBlueAlpha);
        
        _texture = GraphicsDevice.CreateTexture(new TextureDescription(result1.Width, result1.Height,
            Format.R8G8B8A8_UNorm, 0, 2, TextureUsage.ShaderResource), PieUtils.Combine(result1.Data, result2.Data));
        GraphicsDevice.GenerateMipmaps(_texture);*/
        
        /*ImageResult result1 = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/awesomeface.png"), ColorComponents.RedGreenBlueAlpha);
        ImageResult result2 = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/BAGELMIP.png"), ColorComponents.RedGreenBlueAlpha);
        
        ImageResult result3 = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/piegfx-logo-square-temp.png"), ColorComponents.RedGreenBlueAlpha);
        ImageResult result4 = ImageResult.FromMemory(File.ReadAllBytes("/home/ollie/Pictures/EVILMIP.png"), ColorComponents.RedGreenBlueAlpha);

        _texture = GraphicsDevice.CreateTexture(
            new TextureDescription(result1.Width, result1.Height, Format.R8G8B8A8_UNorm, 2, 2,
                TextureUsage.ShaderResource), PieUtils.Combine(result1.Data, result2.Data, result3.Data, result4.Data));
        
        GraphicsDevice.GenerateMipmaps(_texture);*/

        DDS dds = new DDS(File.ReadAllBytes("/home/ollie/Pictures/DDS/24bitcolor-RGBA8.dds"));
        
        Console.WriteLine(dds.MipLevels);
        Console.WriteLine(dds.Size);

        _texture = GraphicsDevice.CreateTexture(
            new TextureDescription(dds.Size.Width, dds.Size.Height, Format.R8G8B8A8_UNorm, dds.MipLevels, 1,
                TextureUsage.ShaderResource), PieUtils.Combine(dds.Bitmaps[0]));
        
        GraphicsDevice.UpdateTexture(_texture, 0, 0, 0, 0, 0, result1.Width, result1.Height, 0, result1.Data);

        /*_texture = GraphicsDevice.CreateTexture(new TextureDescription(dds.Size.Width, dds.Size.Height,
            Format.R8G8B8A8_UNorm, dds.MipLevels, 1, TextureUsage.ShaderResource));

        int width = dds.Size.Width;
        int height = dds.Size.Height;

        for (int i = 0; i < dds.MipLevels; i++)
        {
            GraphicsDevice.UpdateTexture(_texture, i, 0, 0, 0, 0, width, height, 0, dds.Bitmaps[0][i]);

            width /= 2;
            height /= 2;

            if (width < 1)
                width = 1;
            if (height < 1)
                height = 1;
        }*/

        //GraphicsDevice.GenerateMipmaps(_texture);

        _samplerState = GraphicsDevice.CreateSamplerState(SamplerStateDescription.LinearRepeat);

        _shader = GraphicsDevice.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, shader, Language.HLSL, "VertexShader"),
            new ShaderAttachment(ShaderStage.Pixel, shader, Language.HLSL, "PixelShader")
        });

        _layout = GraphicsDevice.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex));

        _depthStencilState = GraphicsDevice.CreateDepthStencilState(DepthStencilStateDescription.Disabled);
        _rasterizerState = GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone);
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);
        
        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
        
        GraphicsDevice.SetShader(_shader);
        GraphicsDevice.SetTexture(0, _texture, _samplerState);
        GraphicsDevice.SetRasterizerState(_rasterizerState);
        GraphicsDevice.SetDepthStencilState(_depthStencilState);
        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes, _layout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        GraphicsDevice.DrawIndexed(6);
    }
}