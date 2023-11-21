using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Pie.Tests.Tests;

// Basic test to make sure Pie works at all.
public class BasicTest : TestBase
{
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private Shader _shader;
    private InputLayout _layout;

    private RasterizerState _rasterizerState;
    private DepthStencilState _depthStencilState;

    protected override void Initialize()
    {
        base.Initialize();

        VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(-1f, -1f, 0.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
            new VertexPositionColor(new Vector3(1f, -1f, 0.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
            new VertexPositionColor(new Vector3(1f, 1f, 0.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
            new VertexPositionColor(new Vector3(-1f, 1f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
        };

        ushort[] indices = new ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        string vertexShader = File.ReadAllText("Content/Shaders/Basic.vert");
        string fragmentShader = File.ReadAllText("Content/Shaders/Basic.frag");

        //byte[] vertexShader = File.ReadAllBytes("Content/Shaders/Basic_vert.spv");
        //byte[] fragmentShader = File.ReadAllBytes("Content/Shaders/Basic_frag.spv");
        
        _vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, vertices);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, indices);

        _shader = GraphicsDevice.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, vertexShader, Language.GLSL),
            new ShaderAttachment(ShaderStage.Fragment, fragmentShader, Language.GLSL)
        }, new []{ new SpecializationConstant(0, 5f) });

        _layout = GraphicsDevice.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32A32_Float, 12, 0, InputType.PerVertex)
        );

        // TODO: Enabling scissor, but without setting a rectangle on Direct3D results in a scissor rectangle of 0x0.
        _rasterizerState = GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone with { ScissorTest = false });
        
        // TODO: Not setting a depth state results in the depth pass always failing in Direct3D.
        _depthStencilState = GraphicsDevice.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);
    }

    private double _totalTime;
    
    protected override void Draw(double dt)
    {
        base.Draw(dt);

        //GraphicsDevice.Scissor = new Rectangle(Point.Empty, GraphicsDevice.Swapchain.Size);
        
        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth | ClearFlags.Stencil, 1, 0);

        _totalTime += dt;

        /*int width = 256;
        int height = 512;
        
        int x = (int) Lerp(0, GraphicsDevice.Swapchain.Size.Width - width, (Math.Sin(_totalTime * 1) + 1) * 0.5);
        int y = (int) Lerp(0, GraphicsDevice.Swapchain.Size.Height - height, (Math.Sin(_totalTime * 4) + 1) * 0.5);

        GraphicsDevice.Scissor = new Rectangle(x, y, width, height);*/

        // TODO: Not setting primitive type in Direct3D (iirc) results in it using points instead.
        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
        
        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionColor.SizeInBytes);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        GraphicsDevice.SetInputLayout(_layout);
        GraphicsDevice.SetShader(_shader);
        GraphicsDevice.SetRasterizerState(_rasterizerState);
        GraphicsDevice.SetDepthStencilState(_depthStencilState);
        
        GraphicsDevice.DrawIndexed(6);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double Lerp(double min, double max, double multiplier) => multiplier * (max - min) + min;
}