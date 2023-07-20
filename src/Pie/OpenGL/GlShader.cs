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

    public unsafe GlShader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        // TODO: Improve this so it doesn't use the temporary handle.
        Handle = Gl.CreateProgram();

        uint[] constIndex = null;
        uint[] constValue = null;
        uint constLength = 0;

        if (SpirvSupported && constants != null && constants.Length > 0)
        {
            constIndex = new uint[constants.Length];
            constValue = new uint[constants.Length];
            constLength = (uint) constants.Length;

            for (int i = 0; i < constants.Length; i++)
            {
                constIndex[i] = constants[i].ID;
                constValue[i] = unchecked((uint) constants[i].Value);
            }
        }
        
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

            // TODO: THIS SHIT IS BROKEN
            // OpenGL appears to have no ability to ignore unused specialization constants, so it just crashes on
            // compilation. Uhhuh. Yep. The API famous for ignoring things is now suddenly unable to ignore anything.
            // OpenGL strikes again!!!!!!!!1111111111111111
            // For now this will remain disabled, so if you want this, sowwy >w<
            if (SpirvSupported)
            {
                fixed (byte* ptr = attachments[i].Spirv)
                {
                    Gl.ShaderBinary(1, &handle, ShaderBinaryFormat.ShaderBinaryFormatSpirV, ptr,
                        (uint) attachments[i].Spirv.Length);
                }

                fixed (uint* pConstI = constIndex)
                fixed (uint* pConstV = constValue)
                    Gl.SpecializeShader(handle, attachments[i].EntryPoint,  constLength, pConstI, pConstV);
            }
            else
            {
                CompilerResult result = Compiler.FromSpirv(IsES ? Language.ESSL : Language.GLSL, attachments[i].Stage,
                    attachments[i].Spirv, attachments[i].EntryPoint, constants);
                if (!result.IsSuccess)
                    throw new PieException(result.Error);

                byte[] source = result.Result;
                fixed (byte* src = source)
                    Gl.ShaderSource(handle, 1, src, source.Length);

                Gl.CompileShader(handle);
            }

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