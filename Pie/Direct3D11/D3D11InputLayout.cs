using System;
using System.Text;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal class D3D11InputLayout : InputLayout
{
    public readonly ID3D11InputLayout Layout;

    public readonly int Stride;
    
    public D3D11InputLayout(InputLayoutDescription[] descriptions)
    {
        InputElementDescription[] iedesc = new InputElementDescription[descriptions.Length];
        int offset = 0;
        for (int i = 0; i < iedesc.Length; i++)
        {
            ref InputElementDescription d = ref iedesc[i];
            ref InputLayoutDescription desc = ref descriptions[i];

            Format fmt = desc.Type switch
            {
                AttributeType.Float => Format.R32_Float,
                AttributeType.Vec2 => Format.R32G32_Float,
                AttributeType.Vec3 => Format.R32G32B32_Float,
                AttributeType.Vec4 => Format.R32G32B32A32_Float,
                _ => throw new ArgumentOutOfRangeException()
            };

            d = new InputElementDescription("TEXCOORD", i, fmt, offset, 0, InputClassification.PerVertexData, 0);

            offset += (int) desc.Type * 4;
        }

        Stride = offset;

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
                case AttributeType.Float:
                    dummyShader.Append("float ");
                    break;
                case AttributeType.Vec2:
                    dummyShader.Append("float2 ");
                    break;
                case AttributeType.Vec3:
                    dummyShader.Append("float3 ");
                    break;
                case AttributeType.Vec4:
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
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Layout.Dispose();
    }
}