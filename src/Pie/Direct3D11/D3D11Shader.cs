using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Pie.ShaderCompiler;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using static Pie.Direct3D11.D3D11GraphicsDevice;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11Shader : Shader
{
    public override bool IsDisposed { get; protected set; }

    private Dictionary<ShaderStage, ComPtr<ID3D11DeviceChild>> _shaders;

    public D3D11Shader(ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        _shaders = new Dictionary<ShaderStage, ComPtr<ID3D11DeviceChild>>();
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];
            
            CompilerResult result = ShaderCompiler.Compiler.FromSpirv(Language.HLSL, attachment.Spirv, constants);
            if (!result.IsSuccess)
                throw new PieException(result.Error);
        
            byte[] hlsl = result.Result;

            switch (attachment.Stage)
            {
                case ShaderStage.Vertex:
                    ComPtr<ID3D10Blob> vShaderBlob = CompileShader(hlsl, "main", "vs_5_0");
                    ComPtr<ID3D11VertexShader> vShader = null;
                    Device.CreateVertexShader(vShaderBlob.GetBufferPointer(), vShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref vShader);
                    _shaders.Add(ShaderStage.Vertex, ComPtr.Downcast<ID3D11VertexShader, ID3D11DeviceChild>(vShader));
                    break;
                case ShaderStage.Fragment:
                    ComPtr<ID3D10Blob> pShaderBlob = CompileShader(hlsl, "main", "ps_5_0");
                    ComPtr<ID3D11PixelShader> pShader = null;
                    Device.CreatePixelShader(pShaderBlob.GetBufferPointer(), pShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref pShader);
                    _shaders.Add(ShaderStage.Fragment, ComPtr.Downcast<ID3D11PixelShader, ID3D11DeviceChild>(pShader));
                    break;
                case ShaderStage.Geometry:
                    ComPtr<ID3D10Blob> gShaderBlob = CompileShader(hlsl, "main", "gs_5_0");
                    ComPtr<ID3D11GeometryShader> gShader = null;
                    Device.CreateGeometryShader(gShaderBlob.GetBufferPointer(), gShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref gShader);
                    _shaders.Add(ShaderStage.Geometry, ComPtr.Downcast<ID3D11GeometryShader, ID3D11DeviceChild>(gShader));
                    break;
                case ShaderStage.Compute:
                    ComPtr<ID3D10Blob> cShaderBlob = CompileShader(hlsl, "main", "vs_5_0");
                    ComPtr<ID3D11ComputeShader> cShader = null;
                    Device.CreateComputeShader(cShaderBlob.GetBufferPointer(), cShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref cShader);
                    _shaders.Add(ShaderStage.Compute, ComPtr.Downcast<ID3D11ComputeShader, ID3D11DeviceChild>(cShader));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        foreach ((_, ComPtr<ID3D11DeviceChild> child) in _shaders)
            child.Dispose();
    }

    public void Use()
    {
        foreach ((ShaderStage stage, ComPtr<ID3D11DeviceChild> child) in _shaders)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    Context.VSSetShader(child.QueryInterface<ID3D11VertexShader>(), null, 0);
                    break;
                case ShaderStage.Fragment:
                    Context.PSSetShader(child.QueryInterface<ID3D11PixelShader>(), null, 0);
                    break;
                case ShaderStage.Geometry:
                    Context.GSSetShader(child.QueryInterface<ID3D11GeometryShader>(), null, 0);
                    break;
                case ShaderStage.Compute:
                    Context.CSSetShader(child.QueryInterface<ID3D11ComputeShader>(), null, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal static ComPtr<ID3D10Blob> CompileShader(byte[] code, string entryPoint, string profile)
    {
        ComPtr<ID3D10Blob> compiledBlob = null;
        ComPtr<ID3D10Blob> errorBlob = null;
        
        fixed (byte* cPtr = code)
        {
            if (!Succeeded(D3DCompiler.Compile(cPtr, (nuint) code.Length, (byte*) null, null,
                    ref Unsafe.NullRef<ID3DInclude>(), entryPoint, profile, 0, 0, ref compiledBlob, ref errorBlob)))
            {
                throw new PieException(
                    $"Failed to compile shader! {Marshal.PtrToStringAnsi((IntPtr) errorBlob.GetBufferPointer())}");
            }
        }

        return compiledBlob;
    }
}