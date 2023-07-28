using System.Drawing;
using System.Numerics;
using Common;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace PieSamples;

public class Main : SampleApplication
{
    private readonly VertexPositionTexture[] _vertices =
    {
        new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
        new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
        new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
        new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0))
    };

    private readonly uint[] _indices =
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    private const string VertexShader = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

layout (binding = 0) uniform Transform
{
    mat4 uTransform;
};

void main()
{
    gl_Position = uTransform * vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

    private const string FragmentShader = @"
#version 450

layout (location = 0) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture1;
layout (binding = 2) uniform sampler2D uTexture2;

void main()
{
    vec4 tex1 = texture(uTexture1, frag_texCoords);
    vec4 tex2 = texture(uTexture2, frag_texCoords);
    out_color = mix(tex1, tex2, 0.2);
}";

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private Shader _shader;
    private InputLayout _inputLayout;

    private Texture _texture1;
    private Texture _texture2;
    private SamplerState _samplerState;

    private GraphicsBuffer _transformBuffer;

    protected override void Initialize()
    {
        _vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, _indices);

        _shader = GraphicsDevice.CreateShader(new []
        {
            new ShaderAttachment(ShaderStage.Vertex, VertexShader),
            new ShaderAttachment(ShaderStage.Fragment, FragmentShader)
        });

        _inputLayout = GraphicsDevice.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // aPosition
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex) // aTexCoords
        );

        TextureDescription textureDesc =
            TextureDescription.Texture2D(0, 0, Format.R8G8B8A8_UNorm, 0, 1, TextureUsage.ShaderResource);
        
        Bitmap b1 = new Bitmap("Content/Textures/container.png");
        textureDesc.Width = b1.Size.Width;
        textureDesc.Height = b1.Size.Height;
        _texture1 = GraphicsDevice.CreateTexture(textureDesc, b1.Data);
        GraphicsDevice.GenerateMipmaps(_texture1);

        Bitmap b2 = new Bitmap("Content/Textures/awesomeface.png");
        textureDesc.Width = b2.Size.Width;
        textureDesc.Height = b2.Size.Height;
        _texture2 = GraphicsDevice.CreateTexture(textureDesc, b2.Data);
        GraphicsDevice.GenerateMipmaps(_texture2);

        _samplerState = GraphicsDevice.CreateSamplerState(SamplerStateDescription.LinearRepeat);

        _transformBuffer = GraphicsDevice.CreateBuffer(BufferType.UniformBuffer, Matrix4x4.Identity, true);
    }

    private double _time;

    protected override void Draw(double dt)
    {
        _time += dt;

        Matrix4x4 transform = Matrix4x4.CreateRotationZ((float) _time) * 
                              Matrix4x4.CreateTranslation(0.5f, -0.5f, 0.0f);
        GraphicsDevice.UpdateBuffer(_transformBuffer, 0, transform);
        
        GraphicsDevice.ClearColorBuffer(0.2f, 0.3f, 0.3f, 1.0f);
        
        GraphicsDevice.SetShader(_shader);
        GraphicsDevice.SetUniformBuffer(0, _transformBuffer);
        GraphicsDevice.SetTexture(1, _texture1, _samplerState);
        GraphicsDevice.SetTexture(2, _texture2, _samplerState);
        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes, _inputLayout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        GraphicsDevice.DrawIndexed((uint) _indices.Length);
    }

    public override void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        
        base.Dispose();
    }

    public Main() : base(new Size(800, 600), "Learn Pie: Chapter 1 Part 4 - Transformations") { }
}