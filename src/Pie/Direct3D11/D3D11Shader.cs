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
    private ComPtr<ID3D11DeviceContext> _context;
    private Dictionary<ShaderStage, ComPtr<ID3D11DeviceChild>> _shaders;
    
    public override bool IsDisposed { get; protected set; }

    public D3D11Shader(ComPtr<ID3D11Device> device, ComPtr<ID3D11DeviceContext> context, ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        _context = context;
        _shaders = new Dictionary<ShaderStage, ComPtr<ID3D11DeviceChild>>();
        
        for (int i = 0; i < attachments.Length; i++)
        {
            ref ShaderAttachment attachment = ref attachments[i];

            CompilerResult result = Compiler.FromSpirv(Language.HLSL, attachment.Stage, attachment.Spirv,
                attachment.EntryPoint, constants);
            if (!result.IsSuccess)
                throw new PieException(result.Error);
        
            byte[] hlsl = result.Result;

            switch (attachment.Stage)
            {
                case ShaderStage.Vertex:
                    ComPtr<ID3D10Blob> vShaderBlob = CompileShader(hlsl, "main", "vs_5_0");
                    ComPtr<ID3D11VertexShader> vShader = null;
                    device.CreateVertexShader(vShaderBlob.GetBufferPointer(), vShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref vShader);
                    _shaders.Add(ShaderStage.Vertex, ComPtr.Downcast<ID3D11VertexShader, ID3D11DeviceChild>(vShader));
                    break;
                case ShaderStage.Fragment:
                    ComPtr<ID3D10Blob> pShaderBlob = CompileShader(hlsl, "main", "ps_5_0");
                    ComPtr<ID3D11PixelShader> pShader = null;
                    device.CreatePixelShader(pShaderBlob.GetBufferPointer(), pShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref pShader);
                    _shaders.Add(ShaderStage.Fragment, ComPtr.Downcast<ID3D11PixelShader, ID3D11DeviceChild>(pShader));
                    break;
                case ShaderStage.Geometry:
                    ComPtr<ID3D10Blob> gShaderBlob = CompileShader(hlsl, "main", "gs_5_0");
                    ComPtr<ID3D11GeometryShader> gShader = null;
                    device.CreateGeometryShader(gShaderBlob.GetBufferPointer(), gShaderBlob.GetBufferSize(),
                        ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref gShader);
                    _shaders.Add(ShaderStage.Geometry, ComPtr.Downcast<ID3D11GeometryShader, ID3D11DeviceChild>(gShader));
                    break;
                case ShaderStage.Compute:
                    ComPtr<ID3D10Blob> cShaderBlob = CompileShader(hlsl, "main", "vs_5_0");
                    ComPtr<ID3D11ComputeShader> cShader = null;
                    device.CreateComputeShader(cShaderBlob.GetBufferPointer(), cShaderBlob.GetBufferSize(),
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
                    _context.VSSetShader(child.QueryInterface<ID3D11VertexShader>(), null, 0);
                    break;
                case ShaderStage.Fragment:
                    _context.PSSetShader(child.QueryInterface<ID3D11PixelShader>(), null, 0);
                    break;
                case ShaderStage.Geometry:
                    _context.GSSetShader(child.QueryInterface<ID3D11GeometryShader>(), null, 0);
                    break;
                case ShaderStage.Compute:
                    _context.CSSetShader(child.QueryInterface<ID3D11ComputeShader>(), null, 0);
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