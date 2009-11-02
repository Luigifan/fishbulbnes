float4x4 matWorldViewProj: WORLDVIEWPROJECTION;
float4x4 matWorld  : WORLD;

uniform extern texture nesTexture;
uniform extern texture nesPalette;

sampler textureSampler = sampler_state
{
    Texture = <nesTexture>;
    mipfilter=None;
    magfilter=Point;
    minfilter=Point;
    MaxMipLevel=1;
    AddressU=Clamp;
    AddressV=Clamp;
};

sampler paletteSampler = sampler_state
{
    Texture = <nesPalette>;
    mipfilter=None;
    magfilter=Point;
    minfilter=Point;
    MaxMipLevel=1;
    AddressU=Clamp;
    AddressV=Clamp;

};

struct VS_OUTPUT
{
    float4 Pos : POSITION;
    float2 tex : TEXCOORD0;
    float4 col : COLOR;
};

struct PS_INPUT {
	float2 uv : TEXCOORD;
};

VS_OUTPUT VS(float4 Pos  : POSITION, float4 col : COLOR, float2 tex : TEXCOORD0)
{
  VS_OUTPUT Out = (VS_OUTPUT)0;
    Out.Pos = mul(Pos,matWorldViewProj);
	// Out.Pos = Pos;
    Out.tex = tex;
    Out.col = col;
  return Out;
}

float4 LookupTilePixel(float2 uv)
{
	float4 result= tex1D(paletteSampler, 
			tex2D(textureSampler, uv).b);
			
	return result;

}

float4 LookupSpritePixel(float2 uv)
{
	float4 result= tex1D(paletteSampler, 
			tex2D(textureSampler, uv).g);
			
	return result;

}


float4 DrawTiles( PS_INPUT input ) : COLOR
{
    // Apply surrounding pixels
    float isSprite = tex2D(textureSampler, input.uv).r;

    float4 color = LookupTilePixel(input.uv);
	color.a = 1.0 - isSprite;
	
    return color;
}

float4 DrawSprites( PS_INPUT input ) : COLOR
{
    // Apply surrounding pixels
    float isSprite = tex2D(textureSampler, input.uv).r;
	float4 color = LookupSpritePixel(input.uv);
	color.a = isSprite;
		
	return color;
    
}

technique DrawTilesTechnique
{
    pass P0
    {
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 DrawTiles();
    }
    
}

technique DrawSpritesTechnique
{
    pass P0
    {
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 DrawSprites();
    }
}

