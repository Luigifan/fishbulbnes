
sampler2D textureSampler : register(s0);
// sampler2D paletteSampler : register(s1);


struct PS_INPUT {
	float2 uv : TEXCOORD;
};

float4 LookupTilePixel(float2 uv)
{
	float4 result= tex2D(textureSampler, uv);
			
	return result;

}

float4 DrawTiles( PS_INPUT input ) : COLOR
{
    return LookupTilePixel(input.uv);
}

