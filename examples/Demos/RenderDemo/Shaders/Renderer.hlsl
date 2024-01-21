struct VSInput
{
    float3 position: POSITION0;
    float2 texCoord: TEXCOORD0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float2 texCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

cbuffer CameraMatrices : register(b0)
{
    float4x4 Projection;
    float4x4 View;
}

cbuffer DrawInfo : register(b1)
{
    float4x4 World;
}

VSOutput Vertex(const in VSInput input)
{
    VSOutput output;

    output.position = mul(Projection, mul(View, mul(World, float4(input.position, 1.0))));
    output.texCoord = input.texCoord;
    
    return output;
}

PSOutput Pixel(const in VSOutput input)
{
    PSOutput output;

    output.color = float4(1.0, 0.5, 0.25, 1.0);

    return output;
}