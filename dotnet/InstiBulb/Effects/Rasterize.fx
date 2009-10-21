sampler2D Input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 Color;
    uv.y = uv.y + (sin((uv.x)*200)*0.01);
    uv.x = uv.x + (cos((uv.y)*200)*0.01);
    Color = tex2D(Input, uv.xy);
    return Color;
}
