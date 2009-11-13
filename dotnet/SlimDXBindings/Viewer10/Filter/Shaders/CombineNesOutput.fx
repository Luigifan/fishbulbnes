Texture2D nesTexture;
Texture2D screenOne;
Texture2D screenTwo;
Texture2D backgroundPic;
Texture2D spriteMask;

float BackgroundBlendFactor;

SamplerState nesSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState linearSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VS_IN
{
	float4 position : POSITION;
	float4 color : COLOR;
	float2 UV: TEXCOORD0;
};

struct PS_IN
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 UV: TEXCOORD0;
};


PS_IN VS( VS_IN vertexShaderIn )
{
	PS_IN vertexShaderOut = (PS_IN)0;
	
	vertexShaderOut.position = vertexShaderIn.position;
	vertexShaderOut.color = vertexShaderIn.color;
	vertexShaderOut.UV = vertexShaderIn.UV;
	
	return vertexShaderOut;
}

float4 PS( PS_IN pixelShaderIn ) : SV_Target
{
    float4 finalColor = screenOne.Sample( linearSampler, pixelShaderIn.UV );
    float4 finalSpriteColor = screenTwo.Sample( linearSampler, pixelShaderIn.UV );
    float4 spriteMaskColor = spriteMask.Sample( linearSampler, pixelShaderIn.UV );
    
	finalSpriteColor.a = 0;
	
	// if this is a foreground sprite, its alpha is the red component (foreground sprite) of the mask
	
	if (spriteMaskColor.r > 0.0 )
	{
		finalSpriteColor.a = spriteMaskColor.r;
	} 
	
	// if this is a background pixel, and there is a sprite here, the sprites alpha is green component (background sprite) of the mask
    if (finalColor.a < 1.0 )
	{
		//float4 bg = backgroundPic.Sample(linearSampler, pixelShaderIn.UV);
		//finalColor = (bg * BackgroundBlendFactor) + (finalColor * (1 - BackgroundBlendFactor));
		if (spriteMaskColor.g > 0)
		{
			finalSpriteColor.a = spriteMaskColor.g;
		}
	} 
	
	float4 result = (finalSpriteColor * finalSpriteColor.a) + (finalColor * (1 - finalSpriteColor.a));
	result.a = 1.0;
	return result;
	
	
    
}


technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
	
}
