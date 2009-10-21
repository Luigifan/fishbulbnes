sampler2D Input : register(s0);
sampler2D Palette : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 Color;
    uv.y = uv.y + (sin((uv.x)*200)*0.01);
    uv.x = uv.x + (cos((uv.y)*200)*0.01);
    Color = tex2D(Input, uv.xy);
    
    	
		// gets the index in the nes' palette
	float i = tex2D(Input,uv.xy).r;
		
		// gets the nes palette entry which is in the 255th scanline (first 32 entries)
	float nesColor = tex2D(Input, float2(i,255*256)).r;
		
		// returns the rgb value
	return tex2D(Palette, float2(nesColor,0));
    
}
