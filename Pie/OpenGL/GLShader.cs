using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GLShader : Shader
{
    public override bool IsDisposed { get; protected set; }

    public uint Handle;
    private Dictionary<string, int> _uniformLocations;

    public static uint BoundHandle;

    private bool _hasShownPerfWarning;

    public unsafe GLShader(ShaderAttachment[] attachments)
    {
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
            fixed (byte* src = attachments[i].Source)
                Gl.ShaderSource(handle, 1, src, attachments[i].Source.Length);
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

        _uniformLocations = new Dictionary<string, int>();
        Gl.GetProgram(Handle, ProgramPropertyARB.ActiveUniforms, out int numUniforms);
        for (uint i = 0; i < numUniforms; i++)
        {
            string name = Gl.GetActiveUniform(Handle, i, out _, out _);
            int location = Gl.GetUniformLocation(Handle, name);
            _uniformLocations.Add(name, location);
        }
    }
    
    public override void Set(string name, bool value)
    {
        BindIfNotBound();
        Gl.Uniform1(GetUniform(name), value ? 1 : 0);
        ReBind();
    }

    public override void Set(string name, int value)
    {
        BindIfNotBound();
        Gl.Uniform1(GetUniform(name), value);
        ReBind();
    }

    public override void Set(string name, float value)
    {
        BindIfNotBound();
        Gl.Uniform1(GetUniform(name), value);
        ReBind();
    }

    public override void Set(string name, Vector2 value)
    {
        BindIfNotBound();
        Gl.Uniform2(GetUniform(name), value);
        ReBind();
    }

    public override void Set(string name, Vector3 value)
    {
        BindIfNotBound();
        Gl.Uniform3(GetUniform(name), value);
        ReBind();
    }

    public override void Set(string name, Vector4 value)
    {
        BindIfNotBound();
        Gl.Uniform4(GetUniform(name), value);
        ReBind();
    }

    public override unsafe void Set(string name, Matrix4x4 value, bool transpose = true)
    {
        BindIfNotBound();
        Gl.UniformMatrix4(GetUniform(name), 1, transpose, (float*) &value);
        ReBind();
    }

    public override void Dispose()
    {
        Gl.DeleteProgram(Handle);
    }

    private int GetUniform(string name)
    {
        if (!_uniformLocations.TryGetValue(name, out int location))
        {
            if (Debug)
            {
                string text =
                    $"Uniform \"{name}\" does not exist in shader. Fix ASAP, an exception will be thrown when debug is disabled.";
                Logging.Log(LogType.Error, text);
                return -1;
            }
            else
                throw new Exception($"Uniform \"{name}\" does not exist in shader.");
        }

        return location;
    }

    private void BindIfNotBound()
    {
        if (BoundHandle != Handle)
        {
            Gl.UseProgram(Handle);
            if (Debug && !_hasShownPerfWarning)
            {
                Logging.Log(LogType.Warning, "For performance reasons, it's recommended before calling Set() that you call GraphicsDevice.SetShader()");
                _hasShownPerfWarning = true;
            }
        }
    }

    private void ReBind()
    {
        if (BoundHandle != Handle)
            Gl.UseProgram(BoundHandle);
    }
}