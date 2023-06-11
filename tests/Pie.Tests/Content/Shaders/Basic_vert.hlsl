struct VSInput
{
    float3 position: POSITION;
    float4 color: COLOR0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float4 color: COLOR0;
};

VSOutput main(VSInput input)
{
    VSOutput output;
    output.position = float4(input.position, 1.0);
    output.color = input.color;
    return output;
}