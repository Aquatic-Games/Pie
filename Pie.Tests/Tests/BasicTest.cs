using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Pie.Tests.Tests;

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

        string vertexShader = File.ReadAllText("Content/Shaders/Basic_vert.hlsl");
        string fragmentShader = File.ReadAllText("Content/Shaders/Basic_pixl.hlsl");

        //byte[] vertexShader = File.ReadAllBytes("Content/Shaders/Basic_vert.spv");
        //byte[] fragmentShader = File.ReadAllBytes("Content/Shaders/Basic_frag.spv");
        
        _vertexBuffer = GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, vertices);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, indices);

        _shader = GraphicsDevice.CreateShader(new ShaderAttachment(ShaderStage.Vertex, vertexShader, Language.HLSL),
            new ShaderAttachment(ShaderStage.Fragment, fragmentShader, Language.HLSL));

        _layout = GraphicsDevice.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32A32_Float, 12, 0, InputType.PerVertex)
        );

        _rasterizerState = GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone with { ScissorTest = true });
        
        // TODO: Not setting a depth state results in the depth pass always failing in Direct3D.
        _depthStencilState = GraphicsDevice.CreateDepthState(DepthStencilStateDescription.LessEqual);
    }

    private double _totalTime;
    
    protected override void Draw(double dt)
    {
        base.Draw(dt);

        //GraphicsDevice.Scissor = new Rectangle(Point.Empty, GraphicsDevice.Swapchain.Size);
        
        GraphicsDevice.Clear(Color.CornflowerBlue, ClearFlags.Depth | ClearFlags.Stencil);

        _totalTime += dt;

        /*int width = 256;
        int height = 512;
        
        int x = (int) Lerp(0, GraphicsDevice.Swapchain.Size.Width - width, (Math.Sin(_totalTime * 1) + 1) * 0.5);
        int y = (int) Lerp(0, GraphicsDevice.Swapchain.Size.Height - height, (Math.Sin(_totalTime * 4) + 1) * 0.5);

        GraphicsDevice.Scissor = new Rectangle(x, y, width, height);*/

        // TODO: Not setting primitive type in Direct3D (iirc) results in it using points instead.
        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);
        
        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionColor.SizeInBytes, _layout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        GraphicsDevice.SetShader(_shader);
        GraphicsDevice.SetRasterizerState(_rasterizerState);
        GraphicsDevice.SetDepthStencilState(_depthStencilState);
        
        GraphicsDevice.DrawIndexed(6);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double Lerp(double min, double max, double multiplier) => multiplier * (max - min) + min;
}