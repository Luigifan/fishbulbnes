Texture2D texture2d;

SamplerState linearSampler
{
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

float4 XBlur( PS_IN pixelShaderIn ) 
{
    float2 dx = float2(1.0 / 256, 0);
    float2 tx = pixelShaderIn.UV.xy - 2*dx;
    
    float4 finalColor = float4(0,0,0,0);

    float4 originalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
    float alpha = originalColor.a;

    for (int i = 0; i < 7; i++)
    {
		finalColor += texture2d.Sample( linearSampler, tx);
		tx += dx;
    }
    finalColor = finalColor / 7;
    
	finalColor.a = alpha;
	return finalColor;
}

float4 YBlur( PS_IN pixelShaderIn ) 
{
    float2 dx = float2(0, 1.0 / 256);
    float2 tx = pixelShaderIn.UV.xy - 2*dx;
    
    float4 finalColor = float4(0,0,0,0);

    float4 originalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
    float alpha = originalColor.a;

    for (int i = 0; i < 7; i++)
    {
		finalColor += texture2d.Sample( linearSampler, tx);
		tx += dx;
    }
    finalColor = finalColor / 7;
    
	finalColor.a = alpha;
	return finalColor;
}

float4 StarBlur( PS_IN pixelShaderIn ) : SV_Target
{
    float4 originalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
    float alpha = originalColor.a;
	float4 result =  (XBlur(pixelShaderIn) + YBlur(pixelShaderIn) + (2 *  originalColor))/4.0;
	result.a = alpha;
	return result;
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, StarBlur() ) );
	}
}
