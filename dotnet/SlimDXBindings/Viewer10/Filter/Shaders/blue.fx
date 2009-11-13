Texture2D texture2d;
Texture2D noiseTex;

SamplerState linearSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

float timer;

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
    float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	return finalColor;	
}

float4 DrawRedderPixels( PS_IN pixelShaderIn ) : SV_Target
{
	float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	finalColor.b *= 2;
	return finalColor;
}

float4 DrawWaves( PS_IN pixelShaderIn ) : SV_Target
{
	float2 wave;
	
	float4 noiseSamp = noiseTex.Sample(linearSampler, (pixelShaderIn.UV * timer).xy);
	float pixelOffset = 0.005 * frac(noiseSamp.a);
	
	
	wave.y = pixelShaderIn.UV.y + (sin((pixelShaderIn.UV.x*15)+timer + noiseSamp.r) * pixelOffset  );
	wave.x = pixelShaderIn.UV.x + (sin((pixelShaderIn.UV.y *5)+timer + noiseSamp.g) * pixelOffset  );
	float4 originalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	float4 finalColor = texture2d.Sample( linearSampler, wave );
	finalColor = (finalColor * 0.4) + (originalColor * 0.6);
	finalColor.a = originalColor.a;
	return finalColor;
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawRedderPixels() ) );
	}
	
}

technique10 Wave
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawWaves() ) );
	}
	
}