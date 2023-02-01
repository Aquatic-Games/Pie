using System;
using System.Text;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11InputLayout : InputLayout
{
    public readonly ID3D11InputLayout Layout;

    public D3D11InputLayout(InputLayoutDescription[] descriptions)
    {
        InputElementDescription[] iedesc = new InputElementDescription[descriptions.Length];
        for (int i = 0; i < iedesc.Length; i++)
        {
            ref InputElementDescription d = ref iedesc[i];
            ref InputLayoutDescription desc = ref descriptions[i];

            Vortice.DXGI.Format fmt = PieUtils.ToDxgiFormat(desc.Format, false);

            d = new InputElementDescription("TEXCOORD", i, fmt, (int) desc.Offset, (int) desc.Slot, (InputClassification) desc.InputType, (int) desc.InputType);
        }

        Descriptions = descriptions;

        Blob dummyBlob = GenerateDummyShader(descriptions);
        Layout = Device.CreateInputLayout(iedesc, dummyBlob);
        dummyBlob.Dispose();
    }

    private Blob GenerateDummyShader(InputLayoutDescription[] descriptions)
    {
        StringBuilder dummyShader = new StringBuilder();
        dummyShader.AppendLine("struct DummyInput {");
        for (int i = 0; i < descriptions.Length; i++)
        {
            ref InputLayoutDescription desc = ref descriptions[i];

            switch (desc.Format)
            {
                case Format.R8_UNorm:
                    dummyShader.Append("float1 ");
                    break;
                case Format.R8G8_UNorm:
                    dummyShader.Append("float2 ");
                    break;
                case Format.R8G8B8A8_UNorm:
                    dummyShader.Append("float4 ");
                    break;
                case Format.B8G8R8A8_UNorm:
                    break;
                case Format.R16G16B16A16_UNorm:
                    dummyShader.Append("half4 ");
                    break;
                case Format.R16G16B16A16_SNorm:
                    dummyShader.Append("half4 ");
                    break;
                case Format.R16G16B16A16_SInt:
                    dummyShader.Append("int4 ");
                    break;
                case Format.R16G16B16A16_UInt:
                    dummyShader.Append("uint4 ");
                    break;
                case Format.R16G16B16A16_Float:
                    dummyShader.Append("half4 ");
                    break;
                case Format.R32G32_SInt:
                    dummyShader.Append("int2 ");
                    break;
                case Format.R32G32_UInt:
                    dummyShader.Append("uint2 ");
                    break;
                case Format.R32G32_Float:
                    dummyShader.Append("float2 ");
                    break;
                case Format.R32G32B32_SInt:
                    dummyShader.Append("int3 ");
                    break;
                case Format.R32G32B32_UInt:
                    dummyShader.Append("uint3 ");
                    break;
                case Format.R32G32B32_Float:
                    dummyShader.Append("float2 ");
                    break;
                case Format.R32G32B32A32_SInt:
                    dummyShader.Append("int4 ");
                    break;
                case Format.R32G32B32A32_UInt:
                    dummyShader.Append("uint4 ");
                    break;
                case Format.R32G32B32A32_Float:
                    dummyShader.Append("float4 ");
                    break;
                case Format.D24_UNorm_S8_UInt:
                    break;
                case Format.R8_SNorm:
                    dummyShader.Append("float1 ");
                    break;
                case Format.R8_SInt:
                    dummyShader.Append("int1 ");
                    break;
                case Format.R8_UInt:
                    dummyShader.Append("uint1 ");
                    break;
                case Format.R8G8_SNorm:
                    dummyShader.Append("float2 ");
                    break;
                case Format.R8G8_SInt:
                    dummyShader.Append("int2 ");
                    break;
                case Format.R8G8_UInt:
                    dummyShader.Append("uint2 ");
                    break;
                case Format.R8G8B8A8_SNorm:
                    dummyShader.Append("float4 ");
                    break;
                case Format.R8G8B8A8_SInt:
                    dummyShader.Append("int4 ");
                    break;
                case Format.R8G8B8A8_UInt:
                    dummyShader.Append("uint4 ");
                    break;
                case Format.R16_UNorm:
                    dummyShader.Append("half1 ");
                    break;
                case Format.R16_SNorm:
                    dummyShader.Append("half1 ");
                    break;
                case Format.R16_SInt:
                    dummyShader.Append("int1 ");
                    break;
                case Format.R16_UInt:
                    dummyShader.Append("uint1 ");
                    break;
                case Format.R16_Float:
                    dummyShader.Append("half1 ");
                    break;
                case Format.R16G16_UNorm:
                    dummyShader.Append("half2 ");
                    break;
                case Format.R16G16_SNorm:
                    dummyShader.Append("half2 ");
                    break;
                case Format.R16G16_SInt:
                    dummyShader.Append("int2 ");
                    break;
                case Format.R16G16_UInt:
                    dummyShader.Append("uint2 ");
                    break;
                case Format.R16G16_Float:
                    dummyShader.Append("half2 ");
                    break;
                case Format.R32_SInt:
                    dummyShader.Append("int1 ");
                    break;
                case Format.R32_UInt:
                    dummyShader.Append("uint1 ");
                    break;
                case Format.R32_Float:
                    dummyShader.Append("float1 ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            dummyShader.AppendLine("var" + i + ": TEXCOORD" + i + ";");
        }

        dummyShader.AppendLine("}; void main(DummyInput input) {}");

        return D3D11Shader.CompileShader(Encoding.UTF8.GetBytes(dummyShader.ToString()), "main", "vs_5_0");
    }

    public override bool IsDisposed { get; protected set; }

    public override InputLayoutDescription[] Descriptions { get; }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Layout.Dispose();
    }
}