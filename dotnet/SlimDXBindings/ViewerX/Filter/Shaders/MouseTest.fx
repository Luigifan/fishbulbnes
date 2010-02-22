Texture2D texture2d;
Texture2D gameOut;

float controlVisibility = 0.0;

float mousePosition[2];

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
	// return float4(1,0,0,1);
    float2 pos; 
    pos.x = (mousePosition[0] * 256) + (pixelShaderIn.UV.x * 8) - 4;
    pos.y = (mousePosition[1] * 256) + (pixelShaderIn.UV.y * 8) - 4;
    pos /= 256;
    float4 gameColor = gameOut.Sample( linearSampler, pos );

	float3 luminance = float3(0.3, 0.59, 0.11);
	float intensity = dot(gameColor.rgb, luminance) ;

    
	return float4(intensity,intensity,intensity,intensity);
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


technique10 Draw
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
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


