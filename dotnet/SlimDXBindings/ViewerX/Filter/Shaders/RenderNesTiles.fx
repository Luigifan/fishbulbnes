float4x4 matWorldViewProj : WORLDVIEWPROJECTION;

Texture2D texture2d;
Texture2D nesOut2;

Texture2D nesPal;
Texture2D chrRam;

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
	
	vertexShaderOut.position = vertexShaderIn.position;
	//note: uncomment this for easy play in various shader designer uis which dont provide a screen aligned quad
	vertexShaderOut.position = mul(vertexShaderIn.position, matWorldViewProj);

	vertexShaderOut.color = vertexShaderIn.color;
	vertexShaderOut.UV = vertexShaderIn.UV;
	
	return vertexShaderOut;
}

float3 FetchColor(int pixel, int tintBits)
{
	return gennedPalette.Load(int3( pixel , tintBits & 7, 0)).rgb;
	
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
	int ppuBankStart; 

	float4 bank = bankSwitches.Load(int3(index, currentBank , 0));
	
	int4 banks = bank * 255.0;
	ppuBankStart = banks[3] << 24 ;
	ppuBankStart = banks[2] << 16 ;
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
	
	
	// int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	
	int curBank =  nesOutdata2[0] * 255.0 ;
	
	for (int i = 0; i < 16; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i, curBank);
		
	}
	
	uint ntBits = finalColor[0] * 255.0;
	
    int vScroll = nesOutdata2[3] * 255.0;
	if (vScroll > 240)
	{
		vScroll = vScroll - 240;
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
	int yLine = currentYPosition - y -1;

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
	// xy.y-=1;
	int ppuByte0 = finalColor.g * 255.0;	
	int ppuByte1 = finalColor.b * 255.0;	

	
	if (!(ppuByte1 & 0x10) || (!(ppuByte1 & 0x4) && xy.x < 8 )) 
		return float4(0,0,0,0);

	
	int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	
	//uint curBank = nesOutData[1] << 8 | nesOutData[0];
	for (int i = 0; i < 16; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i,  bnk[0]);
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
					return float4(0,0,0,0);
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
	return float4(0,0,0,0);
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

	int pixel = rVal[ntPixel & 3] * 255;
	// return colorPal[pixel & 63];
	return float4(FetchColor(rVal[ntPixel & 3] * 255, 0), alpha );
}

float4 DrawTogetherFromRAM(PS_IN pixelShaderIn) : SV_Target
{
	float4 tile = DrawTilesFromRAM(pixelShaderIn);
	float4 sprite = DrawSpritesFromRAM(pixelShaderIn);
	
	if (sprite.a == 1.0)
	{
		return sprite;
	} else
	{
		return tile;
	}
	
}

int nametable;
int patterntable ;

float4 DumpNameTable(PS_IN pixelShaderIn) : SV_Target
{
	int3 texPos = int3( pixelShaderIn.UV * 255.0,0);

	float4 nesOutdata2 = nesOut2.Load(texPos);	
	int2 bnk = int2(nesOutdata2[0] * 255.0, nesOutdata2[1] * 255.0);
	//uint curBank = nesOutData[1] << 8 | nesOutData[0];
	for (int i = 0; i < 16; ++i)
	{
		// hack: right now i'm really only tracking 256 switches per frame,
		// should be as many as needed though
		ppuBankStarts[i] = PPUBankStarts(i, bnk);
	}	
	
	uint ntPixel = 	GetNameTablePixel(texPos.x,	texPos.y, 4096 * patterntable, 
		 0x400 * nametable, 0, 0);
	float r = ntPixel / 32.0;
	// this lookup is 8 columns wide
	// get the nes palette entry (will contain 4 values)
	//float4 rVal = nesPal.Sample(palSampler, palAddy );	
	float4 rVal = nesPal.Load(int3(ntPixel/4, 0,0));

	float alpha = ntPixel & 3 ? 1.0 : 0.0;
	return float4(FetchColor(rVal[ntPixel & 3] * 255, 0), alpha );
}

float4 CreateTileMask( PS_IN pixelShaderIn ) : SV_Target
{
	int ntPixel = GetTilePixel(pixelShaderIn.UV);

	if ((ntPixel & 3) > 0)
		return float4(1,0,0,1);
	else
		return float4 (0,0,0,0);

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

technique10 RenderSpritesRAM
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawSpritesFromRAM() ) );
	}
	
}

technique10 RenderTogetherRAM
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, DrawTogetherFromRAM() ) );
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
