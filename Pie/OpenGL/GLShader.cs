using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Pie.ShaderCompiler;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GlGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GlShader : Shader
{
    public override bool IsDisposed { get; protected set; }

    public uint Handle;
    
    // TODO: WTF??
    public static uint BoundHandle;

    public unsafe GlShader(ShaderAttachment[] attachments)
    {
        // TODO: Improve this so it doesn't use the temporary handle.
        Handle = Gl.CreateProgram();
        for (int i = 0; i < attachments.Length; i++)
        {
            ShaderType type = attachments[i].Stage switch
            {
                ShaderStage.Vertex => ShaderType.VertexShader,
                ShaderStage.Fragment => ShaderType.FragmentShader,
                ShaderStage.Geometry => ShaderType.GeometryShader,
                ShaderStage.Compute => ShaderType.ComputeShader,
                _ => throw new ArgumentOutOfRangeException()
            };

            uint handle = Gl.CreateShader(type);
            attachments[i].TempHandle = handle;
            
            CompilerResult result = Compiler.FromSpirv(Language.GLSL, attachments[i].Spirv);
            if (!result.IsSuccess)
                throw new PieException(result.Error);
            
            byte[] source = result.Result;
            fixed (byte* src = source)
                Gl.ShaderSource(handle, 1, src, source.Length);
            
            Gl.CompileShader(handle);
            Gl.GetShader(handle, ShaderParameterName.CompileStatus, out int compStatus);
            if (compStatus != (int) GLEnum.True)
            {
                throw new PieException($"Error compiling {attachments[i].Stage.ToString().ToLower()} shader!\n" +
                                       Gl.GetShaderInfoLog(handle));
            }
            
            Gl.AttachShader(Handle, handle);
        }
        
        Gl.LinkProgram(Handle);
        Gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int linkStatus);
        if (linkStatus != (int) GLEnum.True)
        {
            throw new PieException("Error occured when linking GL shader program.\n" + Gl.GetProgramInfoLog(Handle));
        }

        for (int i = 0; i < attachments.Length; i++)
        {
            Gl.DetachShader(Handle, attachments[i].TempHandle);
            Gl.DeleteShader(attachments[i].TempHandle);
        }
    }

    public override void Dispose()
    {
        Gl.DeleteProgram(Handle);
    }
}