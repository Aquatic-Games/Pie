using System.Numerics;
using Common;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace PieSamples;

public class Main : SampleApplication
{
    // The quad vertices. They look like this:
    // 0 --------- 1   A quad is made of 2 right-angled triangles.
    // |        /  |   Take a look at the 4 numbers at each corner - these numbers relate to an index in the array 
    // |      /    |   In our vertices, we not only define their position, but also their color.
    // |    /      |   The position is in **clip space**, where (X: -1, Y: -1) is the bottom left, and (X: 1, Y: 1)
    // |  /        |   is the top right of the window.
    // 3 --------- 2
    private readonly VertexPositionColor[] _vertices =
    {
        new VertexPositionColor(new Vector3(0.5f, 0.5f, 0), new Vector4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3(0.5f, -0.5f, 0), new Vector4(0, 1, 0, 1)),
        new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0), new Vector4(0, 0, 1, 1)),
        new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), new Vector4(0, 0, 0, 1))
    };

    // The quad indices. These relate to a given index in our vertices array above.
    private readonly uint[] _indices =
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    // Our vertex shader code. This will help determine where on the screen each vertex is placed.
    private const string VertexShader = @"
#version 450

layout (location = 0) in vec3 aPosition; // Our position attribute, at location 0.
layout (location = 1) in vec4 aColor;    // Our color attribute, at location 1.

layout (location = 0) out vec4 frag_color; // This value will go to the fragment shader, at location 0.

void main()
{
    // Set the output position to the input position. Currently we aren't doing anything with the input value.
    // The 1.0 isn't strictly needed here, but is a habit you should get into, as it will screw up matrix multiplication
    // if you forget! (Covered in a later tutorial)
    gl_Position = vec4(aPosition, 1.0);

    // Again, we do nothing special with the color. We just output it directly to the fragment shader.
    frag_color = aColor;
}";

    // Our fragment shader code. This will help determine what each pixel will look like.
    private const string FragmentShader = @"
#version 450

layout (location = 0) in vec4 frag_color; // Our input color from the vertex shader.

layout (location = 0) out vec4 out_color; // The output of the fragment shader.

void main()
{
    // We're not doing anything special here, just set the output color to the input color.
    out_color = frag_color;
}";

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private Shader _shader;
    private InputLayout _inputLayout;

    protected override void Initialize()
    {
        // Create our vertex and index buffers using the respective arrays.
        _vertexBuffer = Device.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.IndexBuffer, _indices);

        // Create our shader from our code above.
        // Pie's native shading language is SPIR-V, however it can compile shaders at runtime if you pass a string.
        _shader = Device.CreateShader(new []
        {
            new ShaderAttachment(ShaderStage.Vertex, VertexShader),
            new ShaderAttachment(ShaderStage.Fragment, FragmentShader)
        });

        // Create an input layout. Input layouts determine how the graphics device will interpret the vertex data, that
        // we uploaded earlier into our vertex array.
        _inputLayout = Device.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // aPosition
            new InputLayoutDescription(Format.R32G32B32_Float, 12, 0, InputType.PerVertex) // aColor
        );
    }

    protected override void Draw(double dt)
    {
        // Set all our values and draw!
        Device.SetShader(_shader);
        Device.SetPrimitiveType(PrimitiveType.TriangleList);
        Device.SetVertexBuffer(0, _vertexBuffer, VertexPositionColor.SizeInBytes, _inputLayout);
        Device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        Device.DrawIndexed((uint) _indices.Length);
    }

    public override void Dispose()
    {
        // Remember to dispose everything!
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        
        base.Dispose();
    }

    public Main(string title) : base(title) { }
}