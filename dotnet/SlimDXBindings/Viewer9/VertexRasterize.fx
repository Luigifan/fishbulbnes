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
    float4 Col : COLOR;
};

struct PS_INPUT {
	float2 uv : TEXCOORD;
};

VS_OUTPUT VS(float4 Pos  : POSITION, float2 tex : TEXCOORD0, float4 col : COLOR)
{
  VS_OUTPUT Out = (VS_OUTPUT)0;
  Out.Pos = mul(Pos,matWorldViewProj);
  Out.tex = tex;
  Out.Col = col;
  //Out.Col = tex1D(paletteSampler, tex2D(textureSampler, tex).b);
  return Out;
}

float4 PS(PS_INPUT input ) : COLOR
{
	return tex2D(textureSampler, input.uv);
}


technique TVertexShaderOnly
{
    pass P0
    {
        // shaders
        VertexShader = compile vs_3_0 VS();
        PixelShader = null;

    }
	
}
