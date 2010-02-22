Texture2D texture2d;
Texture2D uiArea;

float4x4 matWorldViewProj : WORLDVIEWPROJECTION;
float4x4 nesTransform;
float4x4 uiTransform;

float controlVisibility = 0.0;

SamplerState pointSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

BlendState DisableBlend { BlendEnable[0] = false; };

BlendState SrcAlphaBlendingAdd
    {
        BlendEnable[0] = TRUE;
        SrcBlend = SRC_ALPHA;
        DestBlend = ONE;
        BlendOp = ADD;
        SrcBlendAlpha = ZERO;
        DestBlendAlpha = ZERO;
        BlendOpAlpha = ADD;
        RenderTargetWriteMask[0] = 0x0F;
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
	
	float4 pos = mul(vertexShaderIn.position, matWorldViewProj);
	vertexShaderOut.position = pos;
	vertexShaderOut.color = vertexShaderIn.color;
	vertexShaderOut.UV = vertexShaderIn.UV;
	
	return vertexShaderOut;
}

float4 DrawNES( PS_IN pixelShaderIn ) : SV_Target
{
    float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
    float4 menuColor = uiArea.Sample( pointSampler, pixelShaderIn.UV );
	return (finalColor * (1 - menuColor.a))
		+ (menuColor * menuColor.a);
	
}

float4 DrawMenu( PS_IN pixelShaderIn ) : SV_Target
{
    float4 finalColor = uiArea.Sample( pointSampler, pixelShaderIn.UV );
	return finalColor ;	
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawNES() ) );
		SetBlendState(DisableBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);

	}


}
