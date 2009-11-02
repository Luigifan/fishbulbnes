Texture2D screenOne;
Texture2D screenTwo;

SamplerState linearSampler
{
	Texture = <screenOne>;
    Filter = ANISOTROPIC;
    AddressU = Wrap;
    AddressV = Wrap;
};


SamplerState spriteSampler
{
	Texture = <screenTwo>;
    Filter = ANISOTROPIC;
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
    float4 finalSpriteColor = screenTwo.Sample( spriteSampler, pixelShaderIn.UV );
	finalColor.a = (1.0 - finalSpriteColor.a);
	return (finalColor * finalColor.a) + (finalSpriteColor * finalSpriteColor.a);	
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
