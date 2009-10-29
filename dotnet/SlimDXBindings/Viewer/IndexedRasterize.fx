float4x4 matWorldViewProj: WORLDVIEWPROJECTION;
float4x4 matWorld  : WORLD;

uniform extern texture nesTexture;
uniform extern texture nesPalette;
uniform extern texture lastTexture;

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

sampler lastTextureSampler = sampler_state
{
    Texture = <lastTexture>;
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

float pixelWidth = 1/255;

float2 PixelKernel[13] =
{
    -6, -6,
    -5, -5,
    -4, -4,  
    -3, -3,
    -2, -2, 
    -1, -1,
     0, 0,
     1, 1,
     2, 2,
     3, 3,
     4, 4,
     5, 5,
     6, 6,
};

static const float BlurWeights[13] = 
{
    0.002216,
    0.008764,
    0.026995,
    0.064759,
    0.120985,
    0.176033,
    0.199471,
    0.176033,
    0.120985,
    0.064759,
    0.026995,
    0.008764,
    0.002216,
};


float blurStrength = 0.8;

// Effect function
float4 EffectProcess( PS_INPUT input ) : COLOR
{
    float4 color;
    
    if (input.uv.y >= (255.0f/256.0f) )
    {
		// this is where sprite ram is copied, which fits nicely into a float4
		//    - subtracting the last from this texture should leave nice velocity vectors 
		//		for each sprite in the a and b components
		return tex2D(textureSampler, input.uv) - tex2D(lastTextureSampler, input.uv);
    }
    else
    {
    
		float drawTile = tex2D(textureSampler, input.uv).r;

		if (drawTile > 0)
		{
			color = tex1D(paletteSampler, tex2D(textureSampler, input.uv).b);
			color.a = 1.0;
		} else 
		{
			color = tex1D(paletteSampler, tex2D(textureSampler, input.uv).g);
			color.a = 1.0;
		}
		return color;
	}
    
}


technique TVertexShaderOnly
{
    pass P0
    {
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_3_0 EffectProcess();
    }
    

}
