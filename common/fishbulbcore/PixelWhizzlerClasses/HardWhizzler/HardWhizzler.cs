using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.PPUClasses;

namespace NES.CPU.PixelWhizzlerClasses
{
    public class HardWhizzler : PixelWhizzler
    {

        int currentPixelInfo0, currentPixelInfo1;

        
        protected override void DrawPixel()
        {
            //if (pixelDevices != null && pixelDevices.PixelICareAbout == vbufLocation)
            //{
            //    int x = currentXPosition, y = currentYPosition;
            //    //int luma = GetPixelLuma(x - 8, y);
            //    //luma += GetPixelLuma(x - 4, y);
            //    //luma += GetPixelLuma(x, y);
            //    //luma += GetPixelLuma(x + 4, y);
            //    //luma += GetPixelLuma(x + 8, y);

            //    //luma += GetPixelLuma(x, y - 8);
            //    //luma += GetPixelLuma(x, y - 4);
            //    //luma += GetPixelLuma(x, y + 4);
            //    //luma += GetPixelLuma(x, y + 8);
            //    //luma /= 9;

            //    pixelDevices.PixelValue = GetPixelLuma(x, y); ;
            //}
            
            if (!hitSprite && (sprite0scanline == currentYPosition))
            {
                if (SpriteZeroTest() && TestNTPixel())
                {
                    hitSprite = true;
                    _PPUStatus = _PPUStatus | 0x40;
                }
            }
            outBuffer[vbufLocation] = currentPixelInfo0;
            rgb32OutBuffer[vbufLocation] = currentPixelInfo1;

        }


        protected  override void WriteToNESPalette(int address, byte data)
        {

            Buffer.BlockCopy(_palette, 0, palCache[currentPalette], 0, 32);
            currentPalette++;

            int palAddress = (address) & 0x1F;
            _palette[palAddress] = data;
            if ((_PPUAddress & 0xFFEF) == 0x3F00)
            {
                _palette[(palAddress ^ 0x10) & 0x1F] = data;
            }
            UpdatePixelInfo();
        }
        
        public override void DrawTo(int cpuClockNum)
        {
            int frClock = (cpuClockNum - lastcpuClock) * 3;
            //// if we are in vblank 
            //if (frameClock < 6820)
            //{
            //    // if the frameclock +frClock is in vblank (< 6820) dont do nothing, just update it
            //    if (frameClock + frClock < 6820)
            //    {
            //        frameClock += frClock;
            //        frClock = 0;
            //    }
            //    //else
            //    //{
            //    //    frClock += frameClock - 6820;
            //    //    frameClock = 6820;
            //    //}
            //}
            for (int i = 0; i < frClock; ++i)
            {
                BumpScanline();
            }
            lastcpuClock = cpuClockNum;
        
        }

        public override void UpdatePixelInfo()
        {

            // base.UpdatePixelInfo();

            int curBank = chrRomHandler.CurrentBank;
            int ntbits = nameTableBits & 0x3;
            int vScroll = lockedVScroll;
            if (lockedVScroll < 0)
            {
                ntbits |= 4;
                vScroll *= -1;
                 //lockedVScroll += 240;
            }

            currentPixelInfo0 = (
                currentPalette << 24 | // a
                _PPUControlByte1 << 16 | // r
                _PPUControlByte0 << 8 | // g
                ntbits
                // b
                );



            currentPixelInfo1 =
                (
                    vScroll << 24 | // a
                    lockedHScroll << 16 |  // r
                    (int)(curBank & 0xFFFF)
                );
        }

        protected override void ClearNESPalette()
        {
            currentPalette = 0;
            Buffer.BlockCopy(_palette, 0, palCache[currentPalette], 0, 32);
            UpdatePixelInfo();
        }


        bool SpriteZeroTest()
        {
            if (!_spritesAreVisible) return false;

            int yLine = 0;
            int xPos = 0;
            int tileIndex = 0;

            NESSprite currSprite = currentSprites[0];
            if (
                currSprite.XPosition > 0
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

                return TestSpritePixel(spritePatternTable, xPos, yLine, currSprite, tileIndex);
            }
            return false;
        }

        bool TestNTPixel()
        {
            if (!_tilesAreVisible) return false;

            int xPosition = currentXPosition, yPosition = currentYPosition;
            // int patternTableIndex = PatternTableIndex;

            int ppuNameTableMemoryStart = nameTableBits * 0x400;
            //yPosition = 1;
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
            // index of this pixels bit in pattern table
            int patternTableEntryIndex = 7 - (xPosition & 7);


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

            int tileRow = (yPosition / 8) % 30;

            int tileNametablePosition = 0x2000 + ppuNameTableMemoryStart + (xPosition / 8) + (tileRow * 32);

            int TileIndex = chrRomHandler.GetPPUByte(0, tileNametablePosition);


            int patternTableYOffset = yPosition & 7;


            int patternEntry = chrRomHandler.GetPPUByte(0, _backgroundPatternTableIndex + (TileIndex * 16) + patternTableYOffset);
            int patternEntryByte2 = chrRomHandler.GetPPUByte(0, _backgroundPatternTableIndex + (TileIndex * 16) + 8 + patternTableYOffset);


            // i want the patternTableEntryIndex'th bit of patternEntry in the 1st bit of pixel
            byte result = (byte)(((patternEntry >> patternTableEntryIndex) & 1)
                | (((patternEntryByte2 >> patternTableEntryIndex) & 1) * 2))
                                ;
            return result != 0;
        }

        bool TestSpritePixel(int patternTableIndex, int x, int y, NESSprite sprite, int tileIndex)
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

            patternEntry = chrRomHandler.GetPPUByte(0, patternTableIndex + tileIndex * 16 + y);
            patternEntryBit2 = chrRomHandler.GetPPUByte(0, patternTableIndex + tileIndex * 16 + y + 8);

            return
                (sprite.FlipX ?
                ((patternEntry >> x) & 0x1) | (((patternEntryBit2 >> x) << 1) & 0x2)
                : ((patternEntry >> 7 - x) & 0x1) | (((patternEntryBit2 >> 7 - x) << 1) & 0x2)) != 0;
        }


    }
}
