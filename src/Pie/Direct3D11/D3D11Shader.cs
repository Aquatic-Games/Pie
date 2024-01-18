using System;
using Pie.ShaderCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace Pie.Direct3D11;

internal sealed class D3D11Shader : Shader
{
    private ID3D11DeviceContext _context;
    private ShaderObject[] _shaders;
    
    public override bool IsDisposed { get; protected set; }

    public D3D11Shader(ID3D11Device device, ID3D11DeviceContext context, ShaderAttachment[] attachments, SpecializationConstant[] constants)
    {
        _context = context;
        _shaders = new ShaderObject[attachments.Length];
        
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
                    Blob vShaderBlob = CompileShader(hlsl, "main", "vs_5_0");
                    ID3D11VertexShader vShader = device.CreateVertexShader(vShaderBlob);
                    _shaders[i] = new ShaderObject(ShaderStage.Vertex, vShader);
                    break;
                case ShaderStage.Fragment:
                    Blob pShaderBlob = CompileShader(hlsl, "main", "ps_5_0");
                    ID3D11PixelShader pShader = device.CreatePixelShader(pShaderBlob);
                    _shaders[i] = new ShaderObject(ShaderStage.Pixel, pShader);
                    break;
                case ShaderStage.Geometry:
                    Blob gShaderBlob = CompileShader(hlsl, "main", "gs_5_0");
                    ID3D11GeometryShader gShader = device.CreateGeometryShader(gShaderBlob);
                    _shaders[i] = new ShaderObject(ShaderStage.Geometry, gShader);
                    break;
                case ShaderStage.Compute:
                    Blob cShaderBlob = CompileShader(hlsl, "main", "cs_5_0");
                    ID3D11ComputeShader cShader = device.CreateComputeShader(cShaderBlob);
                    _shaders[i] = new ShaderObject(ShaderStage.Vertex, cShader);
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
        foreach (ShaderObject obj in _shaders)
            obj.Shader.Dispose();
    }

    public void Use()
    {
        foreach (ShaderObject obj in _shaders)
        {
            switch (obj.Stage)
            {
                case ShaderStage.Vertex:
                    _context.VSSetShader((ID3D11VertexShader) obj.Shader);
                    break;
                case ShaderStage.Fragment:
                    _context.PSSetShader((ID3D11PixelShader) obj.Shader);
                    break;
                case ShaderStage.Geometry:
                    _context.GSSetShader((ID3D11GeometryShader) obj.Shader);
                    break;
                case ShaderStage.Compute:
                    _context.CSSetShader((ID3D11ComputeShader) obj.Shader);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal static Blob CompileShader(byte[] code, string entryPoint, string profile)
    {
        /*fixed (byte* cPtr = code)
        {
            if (!Succeeded(D3DCompiler.Compile(cPtr, (nuint) code.Length, (byte*) null, null,
                    ref Unsafe.NullRef<ID3DInclude>(), entryPoint, profile, 0, 0, ref compiledBlob, ref errorBlob)))
            {
                throw new PieException(
                    $"Failed to compile shader! {Marshal.PtrToStringAnsi((IntPtr) errorBlob.GetBufferPointer())}");
            }
        }*/

        if (Vortice.D3DCompiler.Compiler
            .Compile(code, entryPoint, "main", profile, out Blob compiledBlob, out Blob errorBlob).Failure)
        {
            throw new PieException("Failed to compile shader: " + errorBlob.AsString());
        }

        return compiledBlob;
    }

    private struct ShaderObject
    {
        public ShaderStage Stage;
        public ID3D11DeviceChild Shader;

        public ShaderObject(ShaderStage stage, ID3D11DeviceChild shader)
        {
            Stage = stage;
            Shader = shader;
        }
    }
}