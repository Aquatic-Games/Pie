struct VSInput
{
    float3 position: POSITION;
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

cbuffer ProjModel : register(b0)
{
    float4x4 projection;
    float4x4 model;
    float4 tint;
}

Texture2D sprite   : register(t1);
SamplerState state : register(s1);

[[vk::constant_id(0)]] const uint test = 0;

VSOutput VertexShader(const in VSInput input)
{
    VSOutput output;

    output.position = mul(projection, mul(model, float4(input.position, 1.0)));
    output.texCoord = input.texCoord;

    return output;
}

PSOutput PixelShader(const in VSOutput input)
{
    PSOutput output;
    
    output.color = sprite.Sample(state, input.texCoord) * tint;

    if (test)
        output.color.rgb = 1.0 - output.color.rgb;

    return output;
}