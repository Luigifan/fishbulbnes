using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{


    public partial class HardWhizzler
    {



        public bool TestNTPixel()
        {
            if (!_tilesAreVisible) return false;

            int xPosition = currentXPosition, yPosition = currentYPosition;
            // int patternTableIndex = PatternTableIndex;

            int ppuNameTableMemoryStart = nameTableMemoryStart;
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


            int patternEntry = chrRomHandler.GetPPUByte(0,_backgroundPatternTableIndex + (TileIndex * 16) + patternTableYOffset);
            int patternEntryByte2 = chrRomHandler.GetPPUByte(0,_backgroundPatternTableIndex + (TileIndex * 16) + 8 + patternTableYOffset);


            // i want the patternTableEntryIndex'th bit of patternEntry in the 1st bit of pixel
            byte result = (byte)(((patternEntry >> patternTableEntryIndex) & 1)
                | (((patternEntryByte2 >> patternTableEntryIndex) & 1) * 2))
                                ;

            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public byte GetNameTablePixelOld(int xPos, int yPos)
        {
            int xPosition = xPos, yPosition = yPos;
            // int patternTableIndex = PatternTableIndex;

            int ppuNameTableMemoryStart = nameTableMemoryStart;
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

            if (result > 0)
            {
                result |= (byte)GetAttributeTableEntry(ppuNameTableMemoryStart, xPosition / 8, yPosition / 8);
            }
            return result;
        }

        private int GetAttributeTableEntry(int ppuNameTableMemoryStart, int i, int j)
        {
            int LookUp = chrRomHandler.GetPPUByte(0, 0x2000 + ppuNameTableMemoryStart + 0x3C0 + (i / 4) + ((j / 4) * 0x8));

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
    }
}
