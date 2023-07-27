using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Common;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;
using Pie.Windowing;

namespace PieSamples;

public class Main : SampleApplication
{
    private Vector3[] _cubePos =
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(2.0f, 5.0f, -15.0f),
        new Vector3(-1.5f, -2.2f, -2.5f),
        new Vector3(-3.8f, -2.0f, -12.3f),
        new Vector3(2.4f, -0.4f, -3.5f),
        new Vector3(-1.7f, 3.0f, -7.5f),
        new Vector3(1.3f, -2.0f, -2.5f),
        new Vector3(1.5f, 0.2f, -1.5f),
        new Vector3(-1.3f, 1.0f, -1.5f)
    };

    private const string VertexShader = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

layout (binding = 0) uniform ProjViewTransform
{
    mat4 uProjection;
    mat4 uView;
    mat4 uTransform;
};

void main()
{
    gl_Position = uProjection * uView * uTransform * vec4(aPosition, 1.0);
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

    private ProjViewTransform _projViewTransform;
    private GraphicsBuffer _transformBuffer;

    private DepthStencilState _depthStencilState;

    private Camera _camera;

    protected override void Initialize()
    {
        _vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, Cube.Vertices);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, Cube.Indices);

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

        _projViewTransform = new ProjViewTransform();
        _transformBuffer = GraphicsDevice.CreateBuffer(BufferType.UniformBuffer, _projViewTransform, true);

        _depthStencilState = GraphicsDevice.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);

        _camera = new Camera(45, Window.Size.Width / (float) Window.Size.Height);
        _camera.Position = new Vector3(0, 0, -3);

        Window.CursorMode = CursorMode.Locked;
    }

    protected override void Update(double dt)
    {
        base.Update(dt);

        const float camSpeed = 20;
        const float mouseSpeed = 0.01f;

        if (Input.KeyDown(Key.W))
            _camera.Position += _camera.Forward * camSpeed * (float) dt;
        if (Input.KeyDown(Key.S))
            _camera.Position -= _camera.Forward * camSpeed * (float) dt;
        if (Input.KeyDown(Key.A))
            _camera.Position -= _camera.Right * camSpeed * (float) dt;
        if (Input.KeyDown(Key.D))
            _camera.Position += _camera.Right * camSpeed * (float) dt;

        _camera.Rotation.X -= Input.MouseDelta.X * mouseSpeed;
        _camera.Rotation.Y -= Input.MouseDelta.Y * mouseSpeed;

        _camera.Rotation.Y = float.Clamp(_camera.Rotation.Y, -MathF.PI / 2, MathF.PI / 2);

        _projViewTransform.Projection = _camera.ProjectionMatrix;
        _projViewTransform.View = _camera.ViewMatrix;

        if (Input.KeyDown(Key.Escape))
            Close();
    }

    protected override void Draw(double dt)
    {
        GraphicsDevice.ClearColorBuffer(new Vector4(0.2f, 0.3f, 0.3f, 1.0f));
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth, 1.0f, 0);
        
        GraphicsDevice.SetShader(_shader);
        GraphicsDevice.SetUniformBuffer(0, _transformBuffer);
        GraphicsDevice.SetTexture(1, _texture1, _samplerState);
        GraphicsDevice.SetTexture(2, _texture2, _samplerState);
        GraphicsDevice.SetDepthStencilState(_depthStencilState);
        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionTextureNormal.SizeInBytes, _inputLayout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UInt);

        for (int i = 0; i < _cubePos.Length; i++)
        {
            Quaternion rotation = Quaternion.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), 20.0f * i);
            _projViewTransform.Transform = Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(rotation)) *
                                           Matrix4x4.CreateTranslation(_cubePos[i]);
            GraphicsDevice.UpdateBuffer(_transformBuffer, 0, _projViewTransform);
            GraphicsDevice.DrawIndexed((uint) Cube.Indices.Length);
        }
    }

    public override void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        
        base.Dispose();
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct ProjViewTransform
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
        public Matrix4x4 Transform;

        public ProjViewTransform()
        {
            Projection = Matrix4x4.Identity;
            View = Matrix4x4.Identity;
            Transform = Matrix4x4.Identity;
        }
    }

    public Main() : base(new Size(800, 600), "Learn Pie: Chapter 1 Part 6 - Camera") { }
}