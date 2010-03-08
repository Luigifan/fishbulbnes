float4x4 matWorldViewProj : WORLDVIEWPROJECTION;

SamplerState linearSampler
{
    Filter = MIN_MAG_MIP_POINT;

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


//
// VERTEX SHADER
//
//
//

PS_IN VS( VS_IN vertexShaderIn )
{
	PS_IN vertexShaderOut = (PS_IN)0;
	
	vertexShaderOut.position = vertexShaderIn.position;
	//note: uncomment this for easy play in various shader designer uis which dont provide a screen aligned quad
	vertexShaderOut.position = mul(vertexShaderIn.position, matWorldViewProj);

	vertexShaderOut.color = vertexShaderIn.color;
	vertexShaderOut.UV = vertexShaderIn.UV;
	
	return vertexShaderOut;
}

float hue ;

float colorAngles[16] = 
{0, // sin =  -1.0f, cos = 0
240, // sin = -0.866, cos = -0.5
210,  // sin = -0.5, cos = -0.86
180,  //
150,
120,
90,
60,
30,
0,
330,
300,
270,
0,
0,
0};

float lo_levels [4] = { -0.12f, 0.00f, 0.31f, 0.72f };
float hi_levels [4] = {  0.40f, 0.68f, 1.00f, 1.00f };
float phases [0x10 + 3] = {
		-1.0f, -0.86602540378443864676372317075294, -0.5f, 0.0f,  0.5f,  0.86602540378443864676372317075294,
		 1.0f,  0.86602540378443864676372317075294,  0.5f, 0.0f, -0.5f, -0.86602540378443864676372317075294,
		-1.0f, -0.86602540378443864676372317075294, -0.5f, 0.0f,  0.5f,  0.86602540378443864676372317075294,
		 1.0f
	};
	
#define TO_ANGLE_SIN( color )   phases [color]
#define TO_ANGLE_COS( color )   phases [(color) + 3]


//76543210
//||||||||
//||||++++- Hue
//||++----- Value
//++------- Unused

float contrast = 0.30;   /* -1 = dark (0.5)       +1 = light (1.5) */
float brightness = 0.1; /* -1 = dark (0.5)       +1 = light (1.5) */

float3x3 YIQToRGBMatrix = 
{
 1.0, 0.9559999, 0.621000051,
 1.0, -0.271999955, -0.647, 
 1.0, -1.10500014, 1.7019999 
};


// 1.0, -9
float3 DecodePixel(int pixel, int tintBits)
{
	int level = pixel >> 4 & 0x03;
	
	float lo = lo_levels [level];
	float hi = hi_levels [level];
	
	int color = pixel & 0x0F;
	if ( color == 0 )
		lo = hi;
	if ( color == 13 )
		hi = lo;
	if ( color > 13 )
		hi = lo = 0.0f;
		
	/* Convert raw waveform to YIQ */
	float sat = (hi - lo) * 0.5f;
	
	float3 yiq = float3((hi + lo) * 0.5f, 
						sin(radians(colorAngles[color] + hue )) * sat, 
						cos( radians(colorAngles[color] + hue) ) * sat * -1);
	
	/* Apply brightness, contrast, and gamma */
	yiq.x *= (float) (contrast * 0.5f) + 1;
	/* adjustment reduces error when using input palette */
	yiq.x += (float) (brightness * 0.5f) - (0.5f / 256.0f);	
	
	if ( tintBits > 0 && pixel <= 13 )
	{
		float atten_mul = 0.79399f;
		float atten_sub = 0.0782838f;
		
		if ( tintBits == 7 )
		{
			yiq.x = yiq.x * (atten_mul * 1.13f) - (atten_sub * 1.13f);
		}
		else
		{
			int tints [8] = { 0, 6, 10, 8, 2, 4, 0, 0 };
			int tint_color = tints [tintBits];
			float sat = hi * (0.5f - atten_mul * 0.5f) + atten_sub * 0.5f;
			yiq.x -= sat * 0.5f;
			if ( tintBits >= 3 && tintBits != 4 )
			{
				/* combined tint bits */
				sat *= 0.6f;
				yiq.x -= sat;
			}
			yiq.y += sin(radians(colorAngles[color] + hue )) * sat;
			yiq.z += cos( radians(colorAngles[color] + hue) ) * sat * -1;
		}
	}
	float3 rgb1 = mul( yiq , YIQToRGBMatrix);
	// note: i need to figure out what to put here to match a common tv sets 2.65 gamma to a monitors 2.2
	float gamma = 1/2.2;
	rgb1 = ((rgb1 * gamma) - gamma) * rgb1 + rgb1;
	return rgb1 ;
}


float4 CreatePalette( PS_IN pixelShaderIn ) : SV_Target
{
	int3 texPos = int3( pixelShaderIn.UV.x * 255.0, pixelShaderIn.UV.y * 7.0 ,0);
	return float4( DecodePixel(texPos.x , texPos.y ),1);
}

technique10 MakePalette
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, CreatePalette() ) );
	}
	
}
