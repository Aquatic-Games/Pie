using System;
using System.Collections.Generic;
using System.Numerics;
using SharpGen.Runtime;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal class D3D11Shader : Shader
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
                    Blob vShader = CompileShader(attachment.Code, "main", "vs_5_0");
                    _shaders.Add(ShaderStage.Vertex, Device.CreateVertexShader(vShader.AsBytes()));
                    break;
                case ShaderStage.Fragment:
                    Blob fShader = CompileShader(attachment.Code, "main", "ps_5_0");
                    _shaders.Add(ShaderStage.Fragment, Device.CreatePixelShader(fShader.AsBytes()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public override void Set(string name, bool value)
    {
        throw new System.NotImplementedException();
    }

    public override void Set(string name, int value)
    {
        throw new System.NotImplementedException();
    }

    public override void Set(string name, float value)
    {
        throw new System.NotImplementedException();
    }

    public override void Set(string name, Vector2 value)
    {
        throw new System.NotImplementedException();
    }

    public override void Set(string name, Vector3 value)
    {
        throw new System.NotImplementedException();
    }

    public override void Set(string name, Vector4 value)
    {
        throw new System.NotImplementedException();
    }

    public override void Set(string name, Matrix4x4 value, bool transpose = true)
    {
        throw new System.NotImplementedException();
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal static Blob CompileShader(string code, string entryPoint, string profile)
    {
        Result res = Compiler.Compile(code, entryPoint, "main", profile, out Blob mainBlob, out Blob errorBlob);
        if (res.Failure)
            throw new PieException("Shader failed to compile: " + errorBlob.AsString());

        return mainBlob;
    }
}