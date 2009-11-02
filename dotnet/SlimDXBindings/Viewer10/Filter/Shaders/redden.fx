Texture2D texture2d;

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
    float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	return finalColor;	
}

float4 DrawRedderPixels( PS_IN pixelShaderIn ) : SV_Target
{
	float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	finalColor.r *= 2;
	return finalColor;
}

float4 CreateLuminance( PS_IN pixelShaderIn ) : SV_Target
{
	float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	float3 luminance = float3(0.3, 0.59, 0.11);
	float intensity = dot(finalColor.rgb, luminance);
	return float4(1.0 * intensity, 0.0, 0.0, finalColor.a);
}


technique10 Redder
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawRedderPixels() ) );
	}
	
}

technique10 RedLumaMap
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, CreateLuminance() ) );
	}
	
}


