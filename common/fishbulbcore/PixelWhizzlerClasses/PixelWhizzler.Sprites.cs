using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {
        bool spriteChanges;
        private bool _spriteCopyHasHappened;

        public bool SpriteCopyHasHappened
        {
            get { return _spriteCopyHasHappened; }
            set { _spriteCopyHasHappened = value; }
        }


        private bool spriteZeroHit;
        private bool isForegroundPixel;
        private NESSprite[] currentSprites;
        private NESSprite[] unpackedSprites;
        
        private int _maxSpritesPerScanline = 8;

        public int MaxSpritesPerScanline
        {
            get { return _maxSpritesPerScanline; }
            set { _maxSpritesPerScanline = value; }
        }


        private byte[] spriteRAM = new byte[256];

        private int[] spriteLine = new int[256];

        private int _spriteAddress;

        public byte[] SpriteRam
        {
            get { return spriteRAM; }
            // allow a setter to implement spriteRAM DMA xfers
        }

        public void CopySprites(ref byte[] source, int copyFrom)
        {
            // should copy 0x100 items from source to spriteRAM, 
            // starting at SpriteAddress, and wrapping around
            // should set spriteDMA flag
            for (int i = 0; i < 0x100; ++i)
            {
                int spriteLocation = (_spriteAddress + i) & 0xFF;
                if (spriteRAM[spriteLocation] != source[copyFrom + i])
                {
                    spriteRAM[spriteLocation] = source[copyFrom + i];
                    unpackedSprites[spriteLocation / 4].Changed = true;
                }
            }
            _spriteCopyHasHappened = true;
            spriteChanges = true;

        }

        public void InitSprites()
        {
            currentSprites = new NESSprite[_maxSpritesPerScanline];
            for (int i = 0; i < _maxSpritesPerScanline; ++i)
            {
                currentSprites[i] = new NESSprite();
            }

            unpackedSprites = new NESSprite[64];

            for (int i = 0; i < 64; ++i)
            {
                unpackedSprites[i] = new NESSprite();
            }

        }

        
        //well, this is somethin
        // which is better than nothin
        public byte GetSpritePixel()
        {
            spriteZeroHit = false;
            isForegroundPixel = false;
            byte result = 0;
            int yLine = 0;
            int xPos = 0;
            int tileIndex = 0;

            for(int i = 0; i < spritesOnThisScanline; ++i)
            {
                NESSprite currSprite = currentSprites[i];
                if (
                    currSprite.XPosition >0 
                    && currentXPosition >= currSprite.XPosition 
                    && currentXPosition < currSprite.XPosition + 8)
                {

                    int spritePatternTable = 0;
                    if ((_PPUControlByte0 & 0x08) == 0x08)
                    {
                        spritePatternTable = 0x1000;
                    }
                    xPos = currentXPosition - currSprite.XPosition;
                    yLine = currentYPosition - currSprite.YPosition - 1;

                    yLine = yLine & (spriteSize - 1);

                    tileIndex = currSprite.TileIndex;

                    if ((_PPUControlByte0 & 0x20) == 0x20)
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

                    result = WhissaSpritePixel(spritePatternTable, xPos, yLine, currSprite, tileIndex);
                    if (result != 0)
                    {
                        if (currSprite.SpriteNumber == 0)
                        {
                            spriteZeroHit = true;
                        }
                        isForegroundPixel = currSprite.Foreground;
                        return (byte) (result | currSprite.AttributeByte);
                    }
                }
            }
            return 0;
        }

        public byte FastGetSpritePixel()
        {
			int i = spriteLine[currentXPosition];
            spriteZeroHit = (i & 256) == 256;
            isForegroundPixel = (i & 512) == 512;
            return (byte)(i | (i >> 16));
        }


        private byte WhissaSpritePixel(int patternTableIndex, int x, int y, NESSprite sprite, int tileIndex)
        {
            // 8x8 tile
            int patternEntry;
            int patternEntryBit2;

            if (sprite.FlipY)
            {
                y = spriteSize - y - 1;
            }

            if (y >= 8)
            {
                y += 8;
            }

            patternEntry = _vidRAM[patternTableIndex + tileIndex * 16 + y];
            patternEntryBit2 = _vidRAM[patternTableIndex + tileIndex * 16 + y + 8];

            return (byte) 
                (sprite.FlipX ?
                ((patternEntry >> x) & 0x1) | (((patternEntryBit2 >> x) << 1) & 0x2)
                : ((patternEntry >> 7 - x) & 0x1) | (((patternEntryBit2 >> 7 - x) << 1) & 0x2));
        }

        private void WhissaSprite(int patternTableIndex, int y, NESSprite sprite, int tileIndex)
        {
            // 8x8 tile
            int patternEntry;
            int patternEntryBit2;

            if (sprite.FlipY)
            {
                y = spriteSize - y - 1;
            }

            if (y >= 8)
            {
                y += 8;
            }

            patternEntry = _vidRAM[patternTableIndex + tileIndex * 16 + y];
            patternEntryBit2 = _vidRAM[patternTableIndex + tileIndex * 16 + y + 8];

            if (sprite.FlipX)
            {
                for (int x = 0; x < 8; ++x)
                {
                    if (sprite.XPosition + x < 256 && spriteLine[sprite.XPosition + x] == 0)
                    {
                        int b =
                            ((patternEntry ) & 0x1) | (((patternEntryBit2) << 1) & 0x2);

                        if (b != 0)
                        {
                            if (sprite.SpriteNumber == 0)
                            {
                                b |= 256;
                            }
                            if (sprite.Foreground)
                            {
                                b |= 512;
                            }
                            b |= sprite.AttributeByte << 16;
                            spriteLine[sprite.XPosition + x] = b;
                        }
                        patternEntry >>=1;
                        patternEntryBit2 >>=1;
                    }
                }
            }
            else 
            {
                for (int x = 7; x >= 0; --x)
                {
                    if (sprite.XPosition + x < 256 && spriteLine[sprite.XPosition + x] == 0)
                    {
                        int b =
                            ((patternEntry ) & 0x1) | (((patternEntryBit2) << 1) & 0x2);

                        if (b != 0)
                        {
                            if (sprite.SpriteNumber == 0)
                            {
                                b |= 256;
                            }
                            if (sprite.Foreground)
                            {
                                b |= 512;
                            }
                            b |= sprite.AttributeByte << 16;
                            spriteLine[sprite.XPosition + x] = b;
                        }
                        patternEntry >>=1;
                        patternEntryBit2 >>=1;
                    }
                }
            }


        }

        int spritesOnThisScanline;

        int spriteSize;
        /// <summary>
        /// populates the currentSpritesXXX arrays with the first 8 visible sprites on the 
        /// denoted scanline. 
        /// </summary>
        /// <param name="scanline">the scanline to preload sprites for</param>
        public unsafe void PreloadSprites(int scanline)
        {
            spritesOnThisScanline = 0;
			Array.Clear(spriteLine,0,256);
//            fixed (int* p = spriteLine)
//			{
//				int* pt = p, pe = p+ 256;
//				while (pt < pe)
//				{
//					*pt++ = 0;
//				}
//				
//			}
//			for (int i = 0; i < 256; ++i)
//            {
//                spriteLine[i] = 0;
//            }
            for (int spriteNum = 0; spriteNum < 0x100; spriteNum += 4)
            {
                int spriteID = ((spriteNum + _spriteAddress) & 0xFF) >> 2;

                int y = unpackedSprites[spriteID].YPosition + 1;

                if (scanline >= y && scanline < y + spriteSize)
                {

                    //currentSprites[spritesOnThisScanline] = unpackedSprites[spriteID];
                    //currentSprites[spritesOnThisScanline].IsVisible = true;
					DrawSpriteLine(unpackedSprites[spriteID], scanline);

                    spritesOnThisScanline++;
                    if (spritesOnThisScanline == _maxSpritesPerScanline)
                    {
                        break;
                    }
                }
            }
            if (spritesOnThisScanline > 7)
                _PPUStatus = _PPUStatus | 0x20;


//            spritesOnThisScanline = currSprite;
        }

        private void DrawSpriteLine(NESSprite currSprite, int scanline)
        {
            int spritePatternTable = 0;
            if ((_PPUControlByte0 & 0x08) == 0x08)
            {
                spritePatternTable = 0x1000;
            }
            int yLine = scanline - currSprite.YPosition - 1;

            yLine = yLine & (spriteSize - 1);

            int tileIndex = currSprite.TileIndex;

            if ((_PPUControlByte0 & 0x20) == 0x20)
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

            WhissaSprite(spritePatternTable, yLine, currSprite, tileIndex);
            //for (int xPos = 0; xPos < 8; ++xPos)
            //{
            //    int pixelNum = currSprite.XPosition + xPos;
            //    if (pixelNum < 256 && spriteLine[pixelNum] == 0)
            //    {
            //        int result = WhissaSpritePixel(spritePatternTable, xPos, yLine, currSprite, tileIndex);
            //        if (result != 0)
            //        {
            //            if (currSprite.SpriteNumber == 0)
            //            {
            //                result |= 256;
            //            }
            //            if (currSprite.Foreground)
            //            {
            //                result |= 512;
            //            }
            //            result |= currSprite.AttributeByte << 16;
            //            spriteLine[pixelNum] = result;
            //        }
            //    }
            //}
        }

        public void UnpackSprites()
        {
            for (int currSprite = 0; currSprite < unpackedSprites.Length; ++currSprite)
            {
                if (unpackedSprites[currSprite].Changed)
                {
                    UnpackSprite(currSprite);
                }
            }
        }

        private void UnpackSprite(int currSprite)
        {
            byte attrByte = spriteRAM[currSprite * 4 + 2];
            unpackedSprites[currSprite].IsVisible = true;
            unpackedSprites[currSprite].AttributeByte = ((attrByte & 0x03) << 2) | 0x10;
            unpackedSprites[currSprite].YPosition = spriteRAM[currSprite * 4];
            unpackedSprites[currSprite].XPosition = spriteRAM[currSprite * 4 + 3];
            unpackedSprites[currSprite].SpriteNumber = currSprite;
            unpackedSprites[currSprite].Foreground = (attrByte & 0x20) != 0x20;
            unpackedSprites[currSprite].FlipX = (attrByte & 0x40) == 0x40;
            unpackedSprites[currSprite].FlipY = (attrByte & 0x80) == 0x80;
            unpackedSprites[currSprite].TileIndex = spriteRAM[currSprite * 4 + 1];
            unpackedSprites[currSprite].Changed = false;
        }
    }
}
