using System;
using System.Drawing;
using System.Numerics;
using System.Text;
using Pie.ShaderCompiler;
using Pie.Utils;
using Pie.Windowing;

namespace Pie.Tests;

public class Main : IDisposable
{
    private Window _window;
    public GraphicsDevice GraphicsDevice;

    public void Run()
    {
        _window = Window.CreateWithGraphicsDevice(new WindowSettings(), GraphicsApi.Vulkan, out GraphicsDevice,
            new GraphicsDeviceOptions(true));

        VertexPositionColor[] vpcs = new[]
        {
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), new Vector4(1, 0, 0, 1)),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), new Vector4(0, 1, 0, 1)),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.0f), new Vector4(0, 0, 1, 1)),
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.0f), new Vector4(0, 0, 0, 1))
        };

        GraphicsDevice.Viewport = new Rectangle(0, 0, 1280, 720);

        uint[] indices = new[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        GraphicsBuffer vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, vpcs);
        GraphicsBuffer indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, indices);

        const string vertex = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

layout (location = 0) out vec4 frag_color;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    frag_color = aColor;
}";

        const string fragment = @"
#version 450

layout (location = 0) in vec4 frag_color;

layout (location = 0) out vec4 out_color;

void main()
{
    out_color = frag_color;
}";

        Shader shader = GraphicsDevice.CreateCrossPlatformShader(new ShaderAttachment(ShaderStage.Vertex, vertex),
            new ShaderAttachment(ShaderStage.Fragment, fragment));

        /*InputLayout layout = GraphicsDevice.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Float3, 0, 0, InputType.PerVertex),
            new InputLayoutDescription("aColor", AttributeType.Float4, 12, 0, InputType.PerVertex));

        RasterizerState rasterizerState = GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone);*/
        
        while (!_window.ShouldClose)
        {
            _window.ProcessEvents();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
            //GraphicsDevice.SetShader(shader);
            //GraphicsDevice.SetRasterizerState(rasterizerState);
            //GraphicsDevice.SetVertexBuffer(0, vertexBuffer, VertexPositionColor.SizeInBytes, layout);
            //GraphicsDevice.SetIndexBuffer(indexBuffer, IndexType.UInt);
            //GraphicsDevice.DrawIndexed((uint) indices.Length);
            
            GraphicsDevice.Present(1);
        }
    }

    public void Dispose()
    {
        GraphicsDevice.Dispose();
        _window.Dispose();
    }
}