float Offsets[9];
float Weights[9];

texture2D SourceTexture;
sampler2D SourceTextureSampler = sampler_state
{
    Texture = <SourceTexture>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VS_OUTPUT
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT Common_VS(float4 Position : POSITION,
    float2 TexCoord : TEXCOORD0)
{
    VS_OUTPUT OUT;
    
    OUT.Position = Position;
    OUT.TexCoord = TexCoord;
    
    return OUT;
}

float4 GaussianBlurH(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color = {0.0f, 0.0f, 0.0f, 0.0f};
    
    for(int i = 0; i < 9; i++)
    {
        float2 t = texCoord + float2(Offsets[i], 0.0f);
        color += (tex2D(SourceTextureSampler, t)
         * Weights[i]);
    }
        
    return float4( color.rgb, 1.0f );
}

float4 GaussianBlurV(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color = {0.0f, 0.0f, 0.0f, 0.0f};
    
    for(int i = 0; i < 9; i++)
    {
        float2 t = texCoord + float2(Offsets[i], 0.0f);
        color += (tex2D(SourceTextureSampler, t))
         * Weights[i]);
    }
        
    return float4( color.rgb, 1.0f );
}

int ShaderIndex = 0;
PixelShader PS[] =
{
    compile ps_2_0 GaussianBlurH(),
    compile ps_2_0 GaussianBlurV()
};

technique GaussianBlur9x9
{
    pass p0
    {
        VertexShader = compile vs_1_1 Common_VS();
        PixelShader = (PS[ShaderIndex]);
    }
}
