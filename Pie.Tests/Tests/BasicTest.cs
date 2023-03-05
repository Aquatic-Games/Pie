using System;
using System.IO;
using System.Text;
using Pie.ShaderCompiler;

namespace Pie.Tests.Tests;

public class BasicTest : TestBase
{
    protected override void Initialize()
    {
        base.Initialize();

        PieLog.DebugLog += (type, message) => Console.WriteLine($"[{type}] {message}");
        
        //GraphicsDevice test = GraphicsDevice.CreateVulkan();

        string testShader = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

        CompilerResult toSpirvResult =
            Compiler.ToSpirv(Stage.Vertex, Language.GLSL, Encoding.UTF8.GetBytes(testShader), "main");

        if (!toSpirvResult.IsSuccess)
            throw new Exception(toSpirvResult.Error);

        CompilerResult toHlslResult = Compiler.FromSpirv(Language.HLSL, toSpirvResult.Result);
        
        if (!toHlslResult.IsSuccess)
            throw new Exception(toHlslResult.Error);
        
        Console.WriteLine(Encoding.UTF8.GetString(toHlslResult.Result));

        //Shader shader = GraphicsDevice.CreateShader(new ShaderAttachment(ShaderStage.Vertex, testShader));
        
        
    }
}