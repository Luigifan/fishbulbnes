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
};

struct PS_INPUT {
	float2 uv : TEXCOORD;
};

VS_OUTPUT VS(float4 Pos  : POSITION, float2 tex : TEXCOORD0)
{
  VS_OUTPUT Out = (VS_OUTPUT)0;
  Out.Pos = mul(Pos,matWorldViewProj);
    Out.tex = tex;
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

float4 LookupPixel(float2 uv)
{
	float4 result= tex1D(paletteSampler, 
			tex2D(textureSampler, uv).b);
			
	return result;

}

float blurStrength = 0.8;

// Effect function
float4 EffectProcess( PS_INPUT input ) : COLOR
{
    // Apply surrounding pixels
    float4 color = LookupPixel(input.uv);
	color.a = 1.0;
    return color;
}

float4 EffectProcess2( PS_INPUT input ) : COLOR
{
    // Apply surrounding pixels
    float4 color = LookupPixel(input.uv - (3 / 255));
	color.a = 0.8;
    return color;
}

technique TVertexShaderOnly
{
    pass P0
    {
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_3_0 EffectProcess();
    }
    
	//pass P1
    //{
     //   // shaders
     //   VertexShader = compile vs_1_1 VS();
//        PixelShader = compile ps_3_0 EffectProcess2();
        
//        AlphaBlendEnable = True;
//		SrcBlend = InvSrcAlpha;
//		DestBlend = SrcAlpha;
//    }
	
}
