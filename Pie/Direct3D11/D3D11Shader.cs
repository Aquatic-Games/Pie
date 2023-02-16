using System;
using System.Collections.Generic;
using System.Numerics;
using SharpGen.Runtime;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11Shader : Shader
{
    public override bool IsDisposed { get; protected set; }

    private Dictionary<ShaderStage, ID3D11DeviceChild> _shaders;

    public D3D11Shader(ShaderAttachment[] attachments)
    {
        _shaders = new Dictionary<ShaderStage, ID3D11DeviceChild>();
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];

            switch (attachment.Stage)
            {
                case ShaderStage.Vertex:
                    Blob vShader = CompileShader(attachment.Source, "main", "vs_5_0");
                    _shaders.Add(ShaderStage.Vertex, Device.CreateVertexShader(vShader.AsBytes()));
                    break;
                case ShaderStage.Fragment:
                    Blob fShader = CompileShader(attachment.Source, "main", "ps_5_0");
                    _shaders.Add(ShaderStage.Fragment, Device.CreatePixelShader(fShader.AsBytes()));
                    break;
                case ShaderStage.Geometry:
                    Blob gShader = CompileShader(attachment.Source, "main", "gs_5_0");
                    _shaders.Add(ShaderStage.Geometry, Device.CreateGeometryShader(gShader.AsBytes()));
                    break;
                case ShaderStage.Compute:
                    Blob cShader = CompileShader(attachment.Source, "main", "cs_5_0");
                    _shaders.Add(ShaderStage.Compute, Device.CreateComputeShader(cShader.AsBytes()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public override void Set(string name, bool value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, int value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, float value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Vector2 value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Vector3 value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Vector4 value)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Set(string name, Matrix4x4 value, bool transpose = true)
    {
        throw new NotSupportedException(
            "Setting uniforms is not supported on Direct3D 11. You must create a uniform/constant buffer instead.");
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        foreach ((_, ID3D11DeviceChild child) in _shaders)
            child.Dispose();
    }

    public void Use()
    {
        foreach ((ShaderStage stage, ID3D11DeviceChild child) in _shaders)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    Context.VSSetShader((ID3D11VertexShader) child);
                    break;
                case ShaderStage.Fragment:
                    Context.PSSetShader((ID3D11PixelShader) child);
                    break;
                case ShaderStage.Geometry:
                    Context.GSSetShader((ID3D11GeometryShader) child);
                    break;
                case ShaderStage.Compute:
                    Context.CSSetShader((ID3D11ComputeShader) child);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal static Blob CompileShader(byte[] code, string entryPoint, string profile)
    {
        Result res = Compiler.Compile(code, entryPoint, "main", profile, out Blob mainBlob, out Blob errorBlob);
        if (res.Failure)
            throw new PieException("Shader failed to compile: " + errorBlob.AsString());

        return mainBlob;
    }
}