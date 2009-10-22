float4x4 matWorldViewProj: WORLDVIEWPROJECTION;
float4x4 matWorld  : WORLD;

uniform extern texture nesTexture;

struct VS_OUTPUT
{
    float4 Pos : POSITION;
    float2 tex : TEXCOORD0;
};

struct PS_INPUT {
	float2 uv : TEXCOORD;
};

sampler textureSampler = sampler_state
{
    Texture = <nesTexture>;
    mipfilter = POINT; 
};

VS_OUTPUT VS(float4 Pos  : POSITION, float2 tex : TEXCOORD0)
{
  VS_OUTPUT Out = (VS_OUTPUT)0;
  Out.Pos = mul(Pos,matWorldViewProj);
    Out.tex = tex;
  return Out;
}


float4 PS(PS_INPUT input ) : COLOR
{
	return tex2D(textureSampler, input.uv);
}

// Effect technique to be used
technique TVertexShaderOnly
{
    pass P0
    {
        // shaders
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 PS();

        // sampler states
        MinFilter[0] = LINEAR;
        MagFilter[0] = LINEAR;
        MipFilter[0] = POINT;

        // set up texture stage states for single texture modulated by diffuse
        ColorOp[0]   = MODULATE;
        ColorArg1[0] = TEXTURE;
        ColorArg2[0] = CURRENT;
        AlphaOp[0]   = DISABLE;

        ColorOp[1]   = DISABLE;
        AlphaOp[1]   = DISABLE;
    }
}
