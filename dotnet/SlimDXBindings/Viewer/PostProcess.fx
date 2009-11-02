float4x4 matWorldViewProj: WORLDVIEWPROJECTION;
float4x4 matWorld  : WORLD;

uniform extern texture nesTexture;
uniform extern texture tilesTexture;
uniform extern texture lastTexture;

float timer;

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
    minfilter = POINT;
    magfilter = POINT;
};

sampler tilesTextureSampler = sampler_state
{
    Texture = <tilesTexture>;
    mipfilter = LINEAR; 
    minfilter = LINEAR;
    magfilter = LINEAR;
};


sampler lastTextureSampler = sampler_state
{
    Texture = <lastTexture>;
    mipfilter = LINEAR; 
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
	float4 color = tex2D(tilesTextureSampler, input.uv);
	float4 sprite = tex2D(textureSampler, input.uv);
	float4 result = (sprite * sprite.a) + (color * color.a);
	result.a = 1.0;
	return result;
}

float4 WAVE(PS_INPUT input) : COLOR
{
	float2 wave;
	wave.y = input.uv.y + (sin((input.uv.x*15)+timer)*0.01);
	wave.x = input.uv.x + (sin((input.uv.y*5)+timer)*0.01);
	float4 c=tex2D(textureSampler,wave);
	return c;
}

float4 BLUR(PS_INPUT input) : COLOR
{
	float4 c=tex2D(textureSampler,input.uv);
	float4 d=tex2D(lastTextureSampler,input.uv);
	
	float4 dlt = c - d;
	dlt = dlt * -0.5;
	dlt.r = dlt.r * 2.0;
	
	float4 result = c + (0.4 * dlt);
	
	result.a = 1.0;
	return result;
}

// Effect technique to be used
technique Plain
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

technique Wave
{
    pass P0
    {
        // shaders
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 WAVE();

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

technique Blur
{
    pass P0
    {
        // shaders
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 BLUR();

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