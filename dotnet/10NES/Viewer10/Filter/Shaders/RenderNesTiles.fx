float4x4 matWorldViewProj : WORLDVIEWPROJECTION;

Texture2D texture2d;
Texture2D nesOut2;

Texture2D nesPal;
Texture2D chrRam;

Texture2D bankSwitches;

int nesRamStart;
int NameTableMemoryStart;
int HScroll;
int VScroll;



SamplerState linearSampler
{
    Filter = MIN_MAG_MIP_POINT;

};

SamplerState palSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Clamp;
    AddressV = Clamp;
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


float4 colorPal[64] = 
{
	float4(0.4726563, 0.4804688, 0.4726563, 1.0), 
	float4(0.046875, 0.1484375, 0.6328125, 1.0), 
	float4(0.15625, 0.0625, 0.6796875, 1.0), 
	float4(0.3671875, 0.04296875, 0.6289063, 1.0), 
	float4(0.5351563, 0.00390625, 0.4609375, 1.0), 
	float4(0.5742188, 0.01953125, 0.1640625, 1.0), 
	float4(0.5703125, 0.0625, 0.04296875, 1.0), 
	float4(0.4335938, 0.1484375, 0.015625, 1.0), 
	float4(0.2773438, 0.2539063, 0.015625, 1.0), 
	float4(0.078125, 0.3398438, 0.015625, 1.0), 
	float4(0.02734375, 0.3671875, 0.04296875, 1.0), 
	float4(0.0078125, 0.3242188, 0.1796875, 1.0), 
	float4(0.01953125, 0.2773438, 0.4257813, 1.0), 
	float4(0, 0, 0, 1.0), 
	float4(0.0078125, 0.0078125, 0.0078125, 1.0), 
	float4(0.0078125, 0.0078125, 0.0078125, 1.0), 
	float4(0.7460938, 0.75, 0.7460938, 1.0), 
	float4(0.0859375, 0.4101563, 0.8710938, 1.0), 
	float4(0.2929688, 0.2539063, 0.9296875, 1.0), 
	float4(0.5546875, 0.1523438, 0.8828125, 1.0), 
	float4(0.765625, 0.09765625, 0.7265625, 1.0), 
	float4(0.828125, 0.125, 0.3867188, 1.0), 
	float4(0.828125, 0.2148438, 0.1289063, 1.0), 
	float4(0.7070313, 0.3515625, 0.05078125, 1.0), 
	float4(0.5390625, 0.4765625, 0.015625, 1.0), 
	float4(0.2265625, 0.5703125, 0.02734375, 1.0), 
	float4(0.078125, 0.6015625, 0.09375, 1.0), 
	float4(0.03515625, 0.5976563, 0.3515625, 1.0), 
	float4(0.03515625, 0.5429688, 0.6601563, 1.0), 
	float4(0.1757813, 0.1835938, 0.1757813, 1.0), 
	float4(0.01171875, 0.01171875, 0.01171875, 1.0), 
	float4(0.01171875, 0.01171875, 0.01171875, 1.0), 
	float4(0.9570313, 0.9609375, 0.96875, 1.0), 
	float4(0.2851563, 0.6953125, 0.96875, 1.0), 
	float4(0.5351563, 0.578125, 0.9882813, 1.0), 
	float4(0.7578125, 0.4726563, 0.9765625, 1.0), 
	float4(0.9140625, 0.4335938, 0.9257813, 1.0), 
	float4(0.953125, 0.4570313, 0.7265625, 1.0), 
	float4(0.9648438, 0.5117188, 0.421875, 1.0), 
	float4(0.9296875, 0.6328125, 0.2695313, 1.0), 
	float4(0.8398438, 0.75, 0.1328125, 1.0), 
	float4(0.5976563, 0.828125, 0.125, 1.0), 
	float4(0.3125, 0.8671875, 0.265625, 1.0), 
	float4(0.2382813, 0.8632813, 0.578125, 1.0), 
	float4(0.1679688, 0.84375, 0.8671875, 1.0), 
	float4(0.3867188, 0.3984375, 0.390625, 1.0), 
	float4(0.015625, 0.015625, 0.015625, 1.0), 
	float4(0.015625, 0.015625, 0.015625, 1.0), 
	float4(0.96875, 0.9726563, 0.9765625, 1.0), 
	float4(0.703125, 0.8671875, 0.9765625, 1.0), 
	float4(0.796875, 0.8164063, 0.9882813, 1.0), 
	float4(0.8710938, 0.7734375, 0.9804688, 1.0), 
	float4(0.9453125, 0.75, 0.9648438, 1.0), 
	float4(0.9609375, 0.7617188, 0.8867188, 1.0), 
	float4(0.9648438, 0.796875, 0.7617188, 1.0), 
	float4(0.9570313, 0.8515625, 0.6757813, 1.0), 
	float4(0.921875, 0.8984375, 0.5976563, 1.0), 
	float4(0.8203125, 0.9257813, 0.609375, 1.0), 
	float4(0.71875, 0.9414063, 0.703125, 1.0), 
	float4(0.6875, 0.9453125, 0.8476563, 1.0), 
	float4(0.6484375, 0.9375, 0.9453125, 1.0), 
	float4(0.78125, 0.7773438, 0.78125, 1.0), 
	float4(0.1953125, 0.953125, 0.03125, 1.0), 
	float4(0.1953125, 0.953125, 0.03125, 1.0),
	//float4(0.01953125, 0.01953125, 0.01953125, 1.0), 
	//float4(0.01953125, 0.01953125, 0.01953125, 1.0),
	
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

float hue = 0;

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


uint4 FetchSpriteRam(int spriteIndex)
{
	float4 ramEntry = texture2d.Load(int3(spriteIndex,  255 , 0));
	return uint4( ramEntry[0] * 255.0,
		ramEntry[1] * 255.0,
		ramEntry[2] * 255.0,
		ramEntry[3] * 255.0);
}

int PPUBankStarts(int index, int currentBank)
{
	uint ppuBankStart; 
	int y = currentBank ;//currentBank /( 16 * 256);
	int x = 0;// (currentBank & (255)) * 16;

	float4 bank = bankSwitches.Load(int3(x + index,
	y,
	0));
	
	uint4 banks = bank * 255.0;
	ppuBankStart = banks[3] << 24 ;
	ppuBankStart |= banks[2] << 16 ;
	ppuBankStart |= banks[1] << 8 ;
	ppuBankStart |= banks[0]  ;

	return ppuBankStart;
}

static int ppuBankStarts[16] = 
	{-1, -1, -1, -1,
	-1, -1, -1, -1,
	-1, -1, -1, -1,
	-1, -1, -1, -1,}
;
	
uint GetByte(uint address)
{

	
	uint bank = address / 0x400;
	
	uint newAddress = ppuBankStarts[bank] + (address & 0x3FF);
	
	uint y = (newAddress / 1024);
	uint x = (newAddress & 1023);
	float4 byte = chrRam.Load(int3(x/4,y,0));
	
	int result = byte[x & 3] * 255;
	return result;
}


int _backgroundPatternTableIndex=4096;

int GetAttributeTableEntry(int ppuNameTableMemoryStart, int i, int j)
{
	int LookUp = GetByte(0x2000 + ppuNameTableMemoryStart + 0x3C0 + (i >> 2) + ((j >> 2) * 0x8)); 

	switch ((i & 2) | (j & 2) * 2)
	{
		case 0:
			return (LookUp << 2) & 12;
		case 2:
			return LookUp & 12;
		case 4:
			return (LookUp >> 2) & 12;
		case 6:
			return (LookUp >> 4) & 12;
	}
	return 0;
}

uint GetNameTablePixel(int xPosition, int yPosition, uint _backgroundPatternTableIndex, uint nameTableMemoryStart, int lockedHScroll, int lockedVScroll)
{
	// current palette index in a
	// tileIndex in r, spriteIndex in g, isSprite in b
	// calculate the address of the nes palette value in the palCache
	
	
	int ppuNameTableMemoryStart = nameTableMemoryStart;
	
	xPosition += lockedHScroll;

	if (xPosition > 255)
	{
		xPosition -= 256;
		// from loopy's doc
		// you can think of bits 0,1,2,3,4 of the vram address as the "x scroll"(*8)
		//that the ppu increments as it draws.  as it wraps from 31 to 0, bit 10 is
		//switched.  you should see how this causes horizontal wrapping between name
		//tables (0,1) and (2,3).

		ppuNameTableMemoryStart = ppuNameTableMemoryStart ^ 0x400;
	}

	yPosition += lockedVScroll;
	
	if (yPosition < 0)
	{
		yPosition += 240;
	}
	if (yPosition >= 240)
	{
		yPosition -= 240;
		ppuNameTableMemoryStart = ppuNameTableMemoryStart ^ 0x800;
	}	
	

	int xTilePosition = xPosition >> 3;

	uint tileRow = ((yPosition >> 3) % 30) << 5;

	int tileNametablePosition = 0x2000 + ppuNameTableMemoryStart 
		+ xTilePosition + tileRow;

	int TileIndex = GetByte(tileNametablePosition);

	int patternTableYOffset = yPosition & 7;

	int patternID = _backgroundPatternTableIndex 
		+ (TileIndex * 16) + patternTableYOffset;

	int patternEntry = GetByte(patternID);
	int patternEntryByte2 = GetByte(patternID + 8);

	int patternTableEntryIndex = 7 - (xPosition & 7);

	int result = (((patternEntry >> patternTableEntryIndex) & 1)
		| (((patternEntryByte2 >> patternTableEntryIndex) & 1) << 1)
					);
					
	if (result != 0) 
	{
		int attributeByte = GetAttributeTableEntry(ppuNameTableMemoryStart, xTilePosition, yPosition >> 3);
		result |= attributeByte;
	}
	
	return result;
	
}

int GetTilePixel(float2 texposition)
{
	int3 texPos = int3( texposition * 255.0,0);
	float4 finalColor = texture2d.Load(texPos);
	float4 nesOutdata2 = nesOut2.Load(texPos);	
	
	uint ppuByte0 = finalColor[1] * 255.0;	
    uint ppuByte1 = finalColor[2] * 255.0;	

	if (!( ppuByte1 & 0x8) || (!(ppuByte1 & 0x2) && texPos.x < 8 )) 
		return 0;
	
	
	int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	//uint curBank = nesOutData[1] << 8 | nesOutData[0];
	for (int i = 0; i < 15; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i, bnk[1]);
	}
	
	uint ntBits = finalColor[0] * 255.0;
	
    int vScroll = nesOutdata2[3] * 255.0;
	if (vScroll > 240)
	{
		vScroll = vScroll - 256;
	}
	if (ntBits & 4) // vscroll is negative
	{
		vScroll *= -1;
	}
	
	uint hScroll = nesOutdata2[2] * 255.0;

	uint nameTableMemoryStart = (0x400 * (ntBits & 3));
	int _backgroundPatternTableIndex = ((ppuByte0 & 0x10) >> 4) * 0x1000;	
	

	
	uint ntPixel =  
	GetNameTablePixel(texPos.x,	texPos.y, _backgroundPatternTableIndex, 
		nameTableMemoryStart, hScroll, vScroll) ;
	
	return ntPixel;
}



float4 DrawTilesFromRAM(PS_IN pixelShaderIn) : SV_Target
{
	float4 finalColor = texture2d.Load( int3( pixelShaderIn.UV *255,0));
	
	uint ntPixel = GetTilePixel(pixelShaderIn.UV);
	float alpha = (ntPixel & 3) > 0 ? 1.0 : 0.0;
	float r = ntPixel / 32.0;
	// this lookup is 8 columns wide
	float2 palAddy = float2( r, finalColor.a  );

	// get the nes palette entry (will contain 4 values)
	//float4 rVal = nesPal.Sample(palSampler, palAddy );	
	float4 rVal = nesPal.Load(int3(ntPixel/4, finalColor[3] * 255,0));

	return float4(DecodePixel(rVal[ntPixel & 3] * 255, 0), alpha );
}

int nametable;
int patterntable;

float4 DumpNameTable(PS_IN pixelShaderIn) : SV_Target
{
	int3 texPos = int3( pixelShaderIn.UV * 255.0,0);

	float4 nesOutdata2 = nesOut2.Load(texPos);	
	int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	//uint curBank = nesOutData[1] << 8 | nesOutData[0];
	for (int i = 0; i < 15; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i, 0);
	}	
	
	uint ntPixel = 	GetNameTablePixel(texPos.x,	texPos.y, 4096 * patterntable, 
		 0x400 * nametable, 0, 0);
	float r = ntPixel / 32.0;
	// this lookup is 8 columns wide
	// get the nes palette entry (will contain 4 values)
	//float4 rVal = nesPal.Sample(palSampler, palAddy );	
	float4 rVal = nesPal.Load(int3(ntPixel/4, 0,0));

	float alpha = ntPixel & 3 ? 1.0 : 0.0;
	return float4(DecodePixel(rVal[ntPixel & 3] * 255, 0), alpha );
}

float4 CreateTileMask( PS_IN pixelShaderIn ) : SV_Target
{
	int ntPixel = GetTilePixel(pixelShaderIn.UV);

	if ((ntPixel & 3) > 0)
		return float4(1,0,0,1);
	else
		return float4 (0,0,0,0);

}


int spriteStart=0;

uint UnpackUINT(float4 data)
{
	uint l = (uint)(data[3] * 255.0) << 24;
	l |= (uint)(data[2] * 255.0) << 16;
	l |= (uint)(data[1] * 255.0) << 8;
	l |= (uint)(data[0] * 255.0);
	return l;
}


technique10 RenderTileRAM
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawTilesFromRAM() ) );
	}
	
}

technique10 DrawNameTable
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DumpNameTable() ) );
	}
	
}
