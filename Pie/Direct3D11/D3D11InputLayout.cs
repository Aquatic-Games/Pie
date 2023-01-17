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

            Format fmt = desc.Type switch
            {
                AttributeType.Int => Format.R32_SInt,
                AttributeType.Int2 => Format.R32G32_SInt,
                AttributeType.Int3 => Format.R32G32B32_SInt,
                AttributeType.Int4 => Format.R32G32B32A32_SInt,
                AttributeType.Float => Format.R32_Float,
                AttributeType.Float2 => Format.R32G32_Float,
                AttributeType.Float3 => Format.R32G32B32_Float,
                AttributeType.Float4 => Format.R32G32B32A32_Float,
                AttributeType.Byte => Format.R8_UInt,
                AttributeType.Byte2 => Format.R8G8_UInt,
                AttributeType.Byte4 => Format.R8G8B8A8_UInt,
                AttributeType.NByte => Format.R8_UNorm,
                AttributeType.NByte2 => Format.R8G8_UNorm,
                AttributeType.NByte4 => Format.R8G8B8A8_UNorm,
                _ => throw new ArgumentOutOfRangeException()
            };

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

            switch (desc.Type)
            {
                case AttributeType.Int:
                    dummyShader.Append("int ");
                    break;
                case AttributeType.Int2:
                    dummyShader.Append("int2 ");
                    break;
                case AttributeType.Int3:
                    dummyShader.Append("int3 ");
                    break;
                case AttributeType.Int4:
                    dummyShader.Append("int4 ");
                    break;
                case AttributeType.Float:
                    dummyShader.Append("float ");
                    break;
                case AttributeType.Float2:
                    dummyShader.Append("float2 ");
                    break;
                case AttributeType.Float3:
                    dummyShader.Append("float3 ");
                    break;
                case AttributeType.Float4:
                    dummyShader.Append("float4 ");
                    break;
                case AttributeType.Byte:
                    dummyShader.Append("int ");
                    break;
                case AttributeType.Byte2:
                    dummyShader.Append("int2 ");
                    break;
                case AttributeType.Byte4:
                    dummyShader.Append("int4 ");
                    break;
                case AttributeType.NByte:
                    dummyShader.Append("float ");
                    break;
                case AttributeType.NByte2:
                    dummyShader.Append("float2 ");
                    break;
                case AttributeType.NByte4:
                    dummyShader.Append("float4 ");
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