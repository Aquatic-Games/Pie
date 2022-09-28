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

    public override uint Stride { get; }

    public D3D11InputLayout(InputLayoutDescription[] descriptions)
    {
        InputElementDescription[] iedesc = new InputElementDescription[descriptions.Length];
        uint offset = 0;
        for (int i = 0; i < iedesc.Length; i++)
        {
            ref InputElementDescription d = ref iedesc[i];
            ref InputLayoutDescription desc = ref descriptions[i];

            Format fmt;
            uint offsetToAdd;
            switch (desc.Type)
            {
                case AttributeType.Int:
                    fmt = Format.R32_SInt;
                    offsetToAdd = 4;
                    break;
                case AttributeType.Int2:
                    fmt = Format.R32G32_SInt;
                    offsetToAdd = 8;
                    break;
                case AttributeType.Int3:
                    fmt = Format.R32G32B32_SInt;
                    offsetToAdd = 12;
                    break;
                case AttributeType.Int4:
                    fmt = Format.R32G32B32A32_SInt;
                    offsetToAdd = 16;
                    break;
                case AttributeType.Float:
                    fmt = Format.R32_Float;
                    offsetToAdd = 4;
                    break;
                case AttributeType.Float2:
                    fmt = Format.R32G32_Float;
                    offsetToAdd = 8;
                    break;
                case AttributeType.Float3:
                    fmt = Format.R32G32B32_Float;
                    offsetToAdd = 12;
                    break;
                case AttributeType.Float4:
                    fmt = Format.R32G32B32A32_Float;
                    offsetToAdd = 16;
                    break;
                case AttributeType.Byte:
                    fmt = Format.R8_UNorm;
                    offsetToAdd = 4;
                    break;
                case AttributeType.Byte2:
                    fmt = Format.R8G8_UNorm;
                    offsetToAdd = 8;
                    break;
                case AttributeType.Byte4:
                    fmt = Format.R8G8B8A8_UNorm;
                    offsetToAdd = 16;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            d = new InputElementDescription("TEXCOORD", i, fmt, (int) offset, 0, InputClassification.PerVertexData, 0);

            offset += offsetToAdd;
        }

        Stride = offset;

        Descriptions = descriptions;

        Blob dummyBlob = GenerateDummyShader(descriptions);
        Layout = Device.CreateInputLayout(iedesc, dummyBlob);
        dummyBlob.Dispose();
    }

    public D3D11InputLayout(uint stride, InputLayoutDescription[] descriptions) : this(descriptions)
    {
        Stride = stride;
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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            dummyShader.AppendLine(desc.Name + ": TEXCOORD" + i + ";");
        }

        dummyShader.AppendLine("}; void main(DummyInput input) {}");

        return D3D11Shader.CompileShader(dummyShader.ToString(), "main", "vs_5_0");
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