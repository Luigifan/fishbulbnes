sampler2D Input : register(s0);
sampler2D Palette : register(s1);
sampler2D NESPalette : register(s2);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	// gets the index in the nes' palette
	float i = tex2D(Input,uv.xy).a;
		
		// gets the nes palette entry which is in the 255th scanline (first 32 entries)
	float nesColor = tex2D(NESPalette, float2(i,0)).a;
		
		// returns the rgb value
	return tex2D(Palette, float2(nesColor,0));
    
}
