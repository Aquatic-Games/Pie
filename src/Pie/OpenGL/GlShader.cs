using System;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using Pie.ShaderCompiler;

namespace Pie.OpenGL;

internal sealed class GlShader : Shader
{
    public override bool IsDisposed { get; protected set; }

    public int Handle;

    public unsafe GlShader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        Handle = GL.CreateProgram();

        int* shaders = stackalloc int[attachments.Length];
        
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];
            
            ShaderType type = attachment.Stage switch
            {
                ShaderStage.Vertex => ShaderType.VertexShader,
                ShaderStage.Fragment => ShaderType.FragmentShader,
                ShaderStage.Geometry => ShaderType.GeometryShader,
                ShaderStage.Compute => ShaderType.ComputeShader,
                _ => throw new ArgumentOutOfRangeException()
            };

            int shader = GL.CreateShader(type);
            shaders[i] = shader;

            CompilerResult result = Compiler.FromSpirv(Language.GLSL, attachment.Stage, attachment.Spirv,
                attachment.EntryPoint, constants);
            byte[] res = result.Result;
            
            GL.ShaderSource(shader, Encoding.UTF8.GetString(res));
            
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int compStatus);
            if (compStatus != (int) All.True)
            {
                throw new PieException($"OpenGL: Failed to compile {attachment.Stage} shader! " +
                                       GL.GetShaderInfoLog(shader));
            }
            
            GL.AttachShader(Handle, shader);
        }
        
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus != (int) All.True)
            throw new PieException("OpenGL: Failed to link program! " + GL.GetProgramInfoLog(Handle));

        for (int i = 0; i < attachments.Length; i++)
        {
            int shader = shaders[i];
            
            GL.DetachShader(Handle, shader);
            GL.DeleteShader(shader);
        }
    }

    public override void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}