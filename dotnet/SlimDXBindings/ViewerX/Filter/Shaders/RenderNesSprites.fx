float4x4 matWorldViewProj : WORLDVIEWPROJECTION;

Texture2D texture2d;
Texture2D nesOut2;

Texture2D nesPal;
Texture2D chrRam;
Texture2D tileStage;

Texture2D bankSwitches;


Texture2D gennedPalette;



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


//
// VERTEX SHADER
//
//
//

PS_IN VS( VS_IN vertexShaderIn )
{
	PS_IN vertexShaderOut = (PS_IN)0;
	
	vertexShaderOut.position = mul(vertexShaderIn.position, matWorldViewProj);

	vertexShaderOut.color = vertexShaderIn.color;
	vertexShaderOut.UV = vertexShaderIn.UV;
	
	return vertexShaderOut;
}


float3 FetchColor(int pixel, int tintBits)
{
	return gennedPalette.Load(int3( pixel & 63, tintBits & 7, 0)).xyz;
	
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

	float4 bank = bankSwitches.Load(int3(index, currentBank , 0));
	
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

int WhissaSpritePixel(int patternTableIndex, int x, int y, bool flipX, bool flipY, int spriteSize, int tileIndex)
{
	// 8x8 tile
	uint patternEntry;
	uint patternEntryBit2;

	if (flipY)
	{
		y = spriteSize - y - 1;
	}

	if (y >= 8)
	{
		y += 8;
	}

	patternEntry = GetByte(patternTableIndex + (tileIndex * 16) + y);
	patternEntryBit2 = GetByte(patternTableIndex + (tileIndex * 16) + y + 8);

	return 
		(flipX ?
		((patternEntry >> x) & 0x1) | (((patternEntryBit2 >> x) << 1) & 0x2)
		: ((patternEntry >> (7 - x)) & 0x1) | (((patternEntryBit2 >> (7 - x)) << 1) & 0x2));
}


bool PeepSprite(int spriteNum, int currentXPosition, int currentYPosition)
{

	// if these are 8x16 sprites, read high and low, draw
	uint4 spriteData = FetchSpriteRam(spriteNum);
	//int spriteData = spriteRam[spriteNum];
	
	uint y = spriteData[0];
	uint x = spriteData[3];
	
	if ( (x > 0 && currentXPosition >= x && currentXPosition < x + 8) )
		{
			return true;
		}
	
	return false;
}



int DrawSprite(int spriteNum, int currentXPosition, int currentYPosition, int ppuByte0, int ppuByte1)
{
	// if these are 8x16 sprites, read high and low, draw
	//uint spriteData = spriteRam[spriteNum];
	int4 spriteData = FetchSpriteRam(spriteNum);
	
	uint x = spriteData[3];//  (spriteData >> 24) & 0xFF ;
	uint attributeByte  = spriteData[2]; // (spriteData >> 16) & 0xFF;
	uint tileIndex = spriteData[1]; // (spriteData >> 8) & 0xFF;
	uint y = spriteData[0]; // spriteData & 0xFF;
	
	int attrColor = ((attributeByte & 0x03) << 2) | 16;
	bool isInFront = (attributeByte & 32) != 32;
	bool flipX = (attributeByte & 0x40) == 0x40;
	bool flipY = (attributeByte & 0x80) == 0x80;
	int spriteSize = ((ppuByte0 & 0x20) == 0x20) ? 16 : 8;
	
	int spritePatternTable = 0;
	if ((ppuByte0 & 0x08) == 0x08)
	{
		spritePatternTable = 0x1000;
	}
	int xPos = currentXPosition - x;
	int yLine = currentYPosition - y ;

	yLine = yLine & (spriteSize - 1);


	if (spriteSize == 16)
	{
		if ((tileIndex & 1) == 1)
		{
			spritePatternTable = 0x1000;
			tileIndex = tileIndex ^ 1;
		}
		else
		{
			spritePatternTable = 0;
		}
	}
	int result = WhissaSpritePixel(spritePatternTable, xPos, yLine, flipX, flipY, spriteSize, tileIndex);
	if (isInFront)
	{
		result |= 128;
	}

	return result | attrColor;
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

bool IsSpriteOnLine(int lineNum, int spriteNum)
{

	if (spriteNum < 32)
	{
		float4 lineData = texture2d.Load(int3(lineNum, 253, 0));
		uint l = UnpackUINT(lineData);

		return (l & (1 << (spriteNum )));
	} else
	{
		float4 lineData = texture2d.Load(int3(lineNum, 254, 0));
		uint l = UnpackUINT(lineData);
		return (l & (1 << (spriteNum - 32 )));
	}
	return false;
}

float4 DrawSpritesFromRAM(PS_IN pixelShaderIn) : SV_Target
{
	float4 finalColor = texture2d.Load( int3( pixelShaderIn.UV * 255.0, 0) );
	float4 nesOutdata2 = nesOut2.Load( int3( pixelShaderIn.UV * 255.0,0) );
	
	float2 xy = (pixelShaderIn.UV) * 255.0;
	xy.y-=1;
	int ppuByte0 = finalColor.g * 255.0;	
	int ppuByte1 = finalColor.b * 255.0;	

	
	if (!(ppuByte1 & 0x10) || (!(ppuByte1 & 0x4) && xy.x < 8 )) 
		return float4(0,0,0,0);

	
	int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	
	uint curBank = bnk[0] ;
	for (int i = 0; i < 16; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i,  curBank);
	}

			

	int spriteCount=0;
	for(int spriteNum=0; spriteNum < 64; ++spriteNum)
	{
		int j;
		int offset=0;
		if (spriteNum < 32)
		{
			j = spriteNum;
		}
		else
		{
			j = spriteNum - 32;
			offset=1;
		}
		
		if (IsSpriteOnLine(xy.y, spriteNum) )
		{
			if (PeepSprite(spriteNum, xy.x, xy.y))
			{
				spriteCount++;
				if (spriteCount > 64) 
					return tileStage.Load(int3(xy.x,xy.y,0));
				int ntPixel = DrawSprite(spriteNum, xy.x , xy.y, ppuByte0, ppuByte1);
				
				float alpha = (ntPixel & 128) ? 1.0 : 0.5;
				
				ntPixel &=31;
				if ((ntPixel & 3) > 0)
				{
					// get the nes palette entry (will contain 4 values)
					float4 rVal = nesPal.Load(int3(ntPixel/4,finalColor.a * 255,0));
					int pixel = rVal[ntPixel & 3] * 255;
					return float4(FetchColor(pixel, 0), alpha);
				}
			}
		}
		
	}
	return tileStage.Load(int3(xy.x,xy.y,0));
}

float4 CreateSpriteMask( PS_IN pixelShaderIn ) : SV_Target
{
	float4 finalColor = texture2d.Load( int3( pixelShaderIn.UV * 255.0, 0) );
	float4 nesOutdata2 = nesOut2.Load( int3( pixelShaderIn.UV * 255.0,0) );
	
	int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	uint curBank = bnk[0];
	for (int i = 0; i < 16; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i,  curBank);
	}
	float2 xy = (pixelShaderIn.UV) * 255.0;
	xy.y-=1;

	int ppuByte0 = finalColor.g * 255.0;
	int ppuByte1 = finalColor.b * 255.0;

	if (!(ppuByte1 & 0x10) || (!(ppuByte1 & 0x4) && xy.x < 8 )) 
		return float4(0,0,0,0);

	
    // first 32 sprites
	int spriteCount=0;

	//for (int spriteNum = spriteStart; spriteNum < spriteStart + 8; ++spriteNum)
	for(int spriteNum=0; spriteNum < 64; ++spriteNum)
	{
		int j;
		int offset=0;
		if (spriteNum < 32)
		{
			j = spriteNum;
		}
		else
		{
			j = spriteNum - 32;
			offset=1;
		}
		
		if (IsSpriteOnLine(xy.y, spriteNum) && PeepSprite(spriteNum, xy.x, xy.y))
		{
			spriteCount++;
			if (spriteCount > 64) 
				return float4(0,0,0,0);
			int ntPixel = DrawSprite(spriteNum, xy.x , xy.y, ppuByte0, ppuByte1);
			if ((ntPixel  & 3) > 0 )
			{
				if (ntPixel > 128)
				{
					return float4(1,0,0, 1.0 );
				} else 
				{
					return float4(0,1,0,1);
				}
			}
		}
	}
	
	return float4(0,0,0,0);
}

float4 DrawSpriteSquaresFromRAM(PS_IN pixelShaderIn) : SV_Target
{
	float4 finalColor = texture2d.Sample( linearSampler, pixelShaderIn.UV );
	int ppuByte0 = finalColor.g * 255.0;	
	int ppuByte1 = finalColor.b * 255.0;	
	
	int tintBits = (ppuByte1 >>5) & 0x7;
	float2 xy = (pixelShaderIn.UV) * 255.0;
	

	int spriteNum=0;
	while (spriteNum++ < 64)
	{

		if (IsSpriteOnLine(xy.y, spriteNum) )
		{
			if (PeepSprite(spriteNum, xy.x, xy.y))
				return float4(FetchColor(spriteNum + 1,0),0);
		}
	}
	return float4(0,0,0,1);
}

technique10 RenderSpriteMask
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, CreateSpriteMask() ) );
	}

}


technique10 RenderSpriteRAM
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawSpritesFromRAM() ) );
	}
	
}


technique10 RenderSpriteSquares
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawSpriteSquaresFromRAM() ) );
	}
	
}

