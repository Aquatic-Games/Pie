struct PSInput
{
    float4 position: SV_Position;
    float4 color: COLOR0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

PSOutput main(PSInput input)
{
    PSOutput output;
    output.color = input.color;
    return output;
}