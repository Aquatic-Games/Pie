using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Pie.Tests.Tests;

public class MapTest : TestBase
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

        _vertexBuffer =
            GraphicsDevice.CreateBuffer(BufferType.VertexBuffer, (uint) (vertices.Length * VertexPositionColor.SizeInBytes), true);
        _indexBuffer = GraphicsDevice.CreateBuffer(BufferType.IndexBuffer, (uint) (indices.Length * sizeof(ushort)), true);

        MappedSubresource vRes = GraphicsDevice.MapResource(_vertexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(vRes.DataPtr, 0, vertices);
        GraphicsDevice.UnmapResource(_vertexBuffer);

        MappedSubresource iRes = GraphicsDevice.MapResource(_indexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(iRes.DataPtr, 0, indices);
        GraphicsDevice.UnmapResource(_indexBuffer);

        /*
        // Should complain that a buffer has been mapped multiple times.
        GraphicsDevice.MapResource(_vertexBuffer, MapMode.Write);
        GraphicsDevice.MapResource(_vertexBuffer, MapMode.Write);
        
        // Should complain that the buffer has not been mapped.
        GraphicsDevice.UnmapResource(_vertexBuffer);
        */

        _shader = GraphicsDevice.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, vertexShader, Language.GLSL),
            new ShaderAttachment(ShaderStage.Fragment, fragmentShader, Language.GLSL)
        }, new[] { new SpecializationConstant(0, 5f) });

        _layout = GraphicsDevice.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32A32_Float, 12, 0, InputType.PerVertex)
        );

        _rasterizerState =
            GraphicsDevice.CreateRasterizerState(RasterizerStateDescription.CullNone with { ScissorTest = false });

        _depthStencilState = GraphicsDevice.CreateDepthStencilState(DepthStencilStateDescription.LessEqual);
    }

    protected override void Draw(double dt)
    {
        base.Draw(dt);

        GraphicsDevice.ClearColorBuffer(Color.CornflowerBlue);
        GraphicsDevice.ClearDepthStencilBuffer(ClearFlags.Depth | ClearFlags.Stencil, 1, 0);

        GraphicsDevice.SetPrimitiveType(PrimitiveType.TriangleList);

        GraphicsDevice.SetVertexBuffer(0, _vertexBuffer, VertexPositionColor.SizeInBytes, _layout);
        GraphicsDevice.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        GraphicsDevice.SetShader(_shader);
        GraphicsDevice.SetRasterizerState(_rasterizerState);
        GraphicsDevice.SetDepthStencilState(_depthStencilState);

        GraphicsDevice.DrawIndexed(6);
    }
}