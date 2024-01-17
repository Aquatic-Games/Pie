using System;
using Pie.ShaderCompiler;
using static Pie.OpenGL.GlGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GlShader : Shader
{
    public override bool IsDisposed { get; protected set; }

    public uint Handle;

    public unsafe GlShader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        Handle = Gl.CreateProgram();

        uint* shaders = stackalloc uint[attachments.Length];
        
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

            uint shader = Gl.CreateShader(type);
            shaders[i] = shader;

            CompilerResult result = Compiler.FromSpirv(Language.GLSL, attachment.Stage, attachment.Spirv,
                attachment.EntryPoint, constants);
            byte[] res = result.Result;
            
            fixed (byte* rPtr = res)
                Gl.ShaderSource(shader, 1, rPtr, res.Length);
            
            Gl.CompileShader(shader);

            Gl.GetShader(shader, ShaderParameterName.CompileStatus, out int compStatus);
            if (compStatus != (int) GLEnum.True)
            {
                throw new PieException($"OpenGL: Failed to compile {attachment.Stage} shader! " +
                                       Gl.GetShaderInfoLog(shader));
            }
            
            Gl.AttachShader(Handle, shader);
        }
        
        Gl.LinkProgram(Handle);

        Gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int linkStatus);
        if (linkStatus != (int) GLEnum.True)
            throw new PieException("OpenGL: Failed to link program! " + Gl.GetProgramInfoLog(Handle));

        for (int i = 0; i < attachments.Length; i++)
        {
            uint shader = shaders[i];
            
            Gl.DetachShader(Handle, shader);
            Gl.DeleteShader(shader);
        }
    }

    public override void Dispose()
    {
        Gl.DeleteProgram(Handle);
    }
}