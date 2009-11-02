using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public class TileDoodler
    {
        private PixelWhizzler _ppu;

        public int XOffset { get; set; }
        public int YOffset { get; set; }

        private NESSprite sprite0 = new NESSprite();

        public TileDoodler(PixelWhizzler ppu)
        {
            _ppu = ppu;

        }

        // get the 8x8 int[] array representing the tile to be drawn at x, y position (of 32x30 tiles)
        // assume no scrolling
        public int[] GetPatternTableEntry(int PatternTable, int TileIndex, int attributeByte)
        {
            // 8x8 tile
            int[] result = new int[64];
            
            for (int i = 0; i < 8; ++i)
            {
                int patternEntry = _ppu.ChrRomHandler.GetPPUByte(0,  PatternTable + TileIndex * 16 + i);
                int patternEntryBit2 = _ppu.ChrRomHandler.GetPPUByte(0, PatternTable + TileIndex * 16 + i + 8);

                for (int bit = 0; bit < 8; ++bit)
                {
                    if ((patternEntry & PixelWhizzler.PowersOfTwo[bit]) != 0)
                    {
                        result[(i * 8) + bit] = 1 | attributeByte;
                    }
                    if ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[bit]) != 0)
                    {
                        result[(i * 8) + bit] |= 2 | attributeByte;
                    }
                }

            }
            return result;
        }

        public int[] GetSprite(int PatternTable, int TileIndex, int attributeByte, bool flipX, bool flipY)
        {
            // 8x8 tile
            int[] result = new int[64];
            int yMultiplyer = 8;
 

            for (int i = 0; i < 8; ++i)
            {
                int patternEntry ;
                int patternEntryBit2;
                if (flipY)
                {
                    patternEntry = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + 7 - i);
                    patternEntryBit2 = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + 7 - i + 8);
                }
                else
                {

                    patternEntry = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + i);
                    patternEntryBit2 = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + i + 8);
                }
                if (flipX)
                {
                    for (int bit = 7; bit >= 0; --bit)
                    {
                        if ((patternEntry & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * yMultiplyer) + 7 - bit] = 1 | attributeByte;
                        }
                        if ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * yMultiplyer) + 7 - bit] |= 2 | attributeByte;
                        }
                    }
                }
                else
                {
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        if ((patternEntry & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * 8) + bit] = 1 | attributeByte;
                        }
                        if ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * 8) + bit] |= 2 | attributeByte;
                        }
                    }
                }

            }
            return result;
        }

        public bool TryGetSprite(int[] result, int PatternTable, int TileIndex, int attributeByte, bool flipX, bool flipY)
        {
            // 8x8 tile
            int yMultiplyer = 8;
            bool hasData = false;

            for (int i = 0; i < 8; ++i)
            {
                int patternEntry;
                int patternEntryBit2;
                if (flipY)
                {
                    patternEntry = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + 7 - i);
                    patternEntryBit2 = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + 7 - i + 8);
                }
                else
                {
                    patternEntry = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + i);
                    patternEntryBit2 = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + i + 8);
                }

                if (flipX)
                {
                    for (int bit = 7; bit >= 0; --bit)
                    {
                        result[(i * yMultiplyer) + 7 - bit] = 0;
                        if ((patternEntry & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * yMultiplyer) + 7 - bit] = 1 | attributeByte;
                            hasData = true;
                        }
                        if ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * yMultiplyer) + 7 - bit] |= 2 | attributeByte;
                            hasData = true;
                        }
                    }
                }
                else
                {
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        result[(i * 8) + bit] = 0;
                        if ((patternEntry & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * 8) + bit] = 1 | attributeByte;
                            hasData = true;
                        }
                        if ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[bit]) != 0)
                        {
                            result[(i * 8) + bit] |= 2 | attributeByte;
                            hasData = true;
                        }
                    }
                }
            }
            return hasData;
        }

        /// <summary>
        /// Gets a 1x8 line from a particular pattern table
        /// </summary>
        /// <param name="LineNumber"></param>
        /// <param name="PatternTable"></param>
        /// <param name="TileIndex"></param>
        /// <param name="attributeByte"></param>
        /// <returns></returns>
        public void GetPatternTableLine(ref int[] result, int startPosition, int LineNumber, int PatternTable, int TileIndex, int attributeByte)
        {
            // 8x8 tile

            int patternEntry = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + LineNumber);
            int patternEntryBit2 = _ppu.VidRAM_GetNTByte(PatternTable + TileIndex * 16 + LineNumber + 8);

            for (int bit = 0; bit < 8; ++bit)
            {
                if ((patternEntry & PixelWhizzler.PowersOfTwo[bit]) != 0)
                {
                    result[(LineNumber * 8) + bit] = 1 | attributeByte;
                }
                if ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[bit]) != 0)
                {
                    result[(LineNumber * 8) + bit] |= 2 | attributeByte;
                }
            }
        }

        void DrawRect(int[] newData, int width, int height, int xPos, int yPos)
        {

            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {

                    int xPosition = xPos + 8 - i;
                    int yPosition = yPos + j;

                    if (xPosition >= 256 || yPosition >= 240) return;
                    _ppu.CurrentFrame[yPosition * 256 + xPosition] = (byte)newData[(j * width) + i];
                }
            }
        }

        void MergeRect(int[] newData, int width, int height, int xPos, int yPos, bool inFront)
        {

            if (inFront)
            {
                MergeRectBehind(newData, width, height, xPos, yPos);
                return;
            }

            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {

                    int xPosition = xPos + 8 - i;
                    int yPosition = yPos + j;

                    if (xPosition >= 256 || yPosition >= 240) return;
                    if (newData[(j * width) + i] != 0)
                    {
                        _ppu.CurrentFrame[yPosition * 256 + xPosition] =
                            (byte)_ppu.VidRAM_GetNTByte((newData[(j * width) + i]) + 0x3F00);
                    }
                }
            }
        }

        void MergeRectBehind(int[] newData, int width, int height, int xPos, int yPos)
        {

            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {

                    int xPosition = xPos + 8 - i;
                    int yPosition = yPos + j;

                    if (xPosition >= 256 || yPosition >= 240) return;
                    if (_ppu.CurrentFrame[yPosition * 256 + xPosition] == _ppu.VidRAM_GetNTByte(0x3F00))
                    {
                        _ppu.CurrentFrame[yPosition * 256 + xPosition] =
                            (byte)_ppu.VidRAM_GetNTByte((newData[(j * width) + i]) + 0x3F00);
                    }
                }
            }
        }

        public void DrawAllTiles()
        {
            if (YOffset > 256) YOffset = YOffset & 0xFF;
            if (XOffset > 256) XOffset = XOffset & 0xFF;

            //_ppu.RawBuffer = new byte[_ppu.RawBuffer.Length + 1];

            int NameTable = 0x2000 + (0x400 * (_ppu.PPUControlByte0 & 0x3));
            int nt2 = (NameTable & 0xC00) / 0x400;
            
            //int PatternTable;
            //if ((_ppu.PPUControlByte0 & 0x10) != 0)
            //    PatternTable = 0x1000;
            //else
            //    PatternTable = 0;

            for (int i = 0; i < 32; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    //int TileIndex = (byte)_ppu.NameTable[_ppu.Mirror[nt2], i + (j * 32)];
                    int TileIndex = (byte)_ppu.VidRAM_GetNTByte(0x2000 + _ppu.NameTableMemoryStart + i + (j * 32));

                    int addToCol = GetAttributeTableEntry(_ppu.NameTableMemoryStart, i, j);
                    DrawRect(GetPatternTableEntry(_ppu.PatternTableIndex, TileIndex, addToCol), 8, 8, (i * 8) + XOffset, (j * 8) + YOffset);

                }
            }
        }

        private int GetAttributeTableEntry(int ppuNameTableMemoryStart, int i, int j)
        {
            //int LookUp = _ppu.NameTable[_ppu.Mirror[nameTableIndex],
            //    0x3C0 + (i / 4) + ((j / 4) * 0x8)];

            int LookUp = _ppu.VidRAM_GetNTByte(0x2000 + ppuNameTableMemoryStart +
                0x3C0 + (i / 4) + ((j / 4) * 0x8));

            int AttribByte = 0;
            switch ((i & 2) | (j & 2) * 2)
            {
                case 0:
                    AttribByte = (LookUp << 2) & 12;
                    break;
                case 2:
                    AttribByte = LookUp & 12;
                    break;
                case 4:
                    AttribByte = (LookUp >> 2) & 12;
                    break;
                case 6:
                    AttribByte = (LookUp >> 4) & 12;
                    break;
            }
            return AttribByte;
        }

        /// <summary>
        /// Returns a pixel 
        /// </summary>
        /// <param name="PatternTable"></param>
        /// <param name="xPosition">X position of pixel (0 to 255)</param>
        /// <param name="yPosition">Y position of pixel (0 to 239)</param>
        /// <returns></returns>
        public byte GetNameTablePixel(int xPosition, int yPosition)
        {
            int ppuNameTableMemoryStart = _ppu.NameTableMemoryStart;
            //yPosition += 1;
            xPosition += _ppu.HScroll;

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
            int xTilePosition = xPosition / 8;
            // index of this pixels bit in pattern table
            int patternTableEntryIndex = 7 - (xPosition & 7);

            yPosition += _ppu.VScroll;
            if (yPosition > 240)
            {
                yPosition -= 241;
                ppuNameTableMemoryStart = ppuNameTableMemoryStart ^ 0x800;
            }

            int yTilePosition = yPosition / 8;

            int patternTableYOffset = yPosition & 7;


            //int mirrorIndexLookup = (nameTableMemoryStart & 0xC00) / 0x400;
            //int TileIndex = (byte)_ppu.NameTable[_ppu.CurrentNameTable, xTilePosition + (yTilePosition * 32)];

            int TileIndex = (byte)_ppu.VidRAM_GetNTByte(0x2000 + ppuNameTableMemoryStart + xTilePosition + ((yTilePosition * 32)));

            int patternEntry = _ppu.VidRAM_GetNTByte(_ppu.PatternTableIndex + TileIndex * 16 + patternTableYOffset);
            int patternEntryByte2 = _ppu.VidRAM_GetNTByte(_ppu.PatternTableIndex + TileIndex * 16 + 8 + patternTableYOffset);

            int attributeByte = GetAttributeTableEntry(ppuNameTableMemoryStart, xTilePosition, yTilePosition);

            // i want the patternTableEntryIndex'th bit of patternEntry in the 1st bit of pixel
            return (byte)(((patternEntry >> patternTableEntryIndex) & 1)
                | (((patternEntryByte2 >> patternTableEntryIndex) & 1) * 2)
                                | attributeByte);
        }

        public bool SpriteZeroHit(int xPosition, int yPosition)
        {
            int y = _ppu.SpriteRam[0];
            int yLine = yPosition % 8;
            int xPos = xPosition % 8;
            if (yPosition > y && yPosition <=  y + 9)
            {
                int tileIndex = _ppu.SpriteRam[1];
                int patternEntry = _ppu.VidRAM_GetNTByte(_ppu.PatternTableIndex + tileIndex * 16 + 7 - yLine);
                int patternEntryBit2 = _ppu.VidRAM_GetNTByte(_ppu.PatternTableIndex + tileIndex * 16 + 7 - yLine + 8);

                if (((patternEntry & PixelWhizzler.PowersOfTwo[xPos]) != 0) || ((patternEntryBit2 & PixelWhizzler.PowersOfTwo[xPos]) != 0))
                {
                    return true;
                }
            }
            return false;
        }

        public void DrawSprite(int spriteNum)
        {
            int spriteAddress = 4 * spriteNum;
            int y = _ppu.SpriteRam[spriteAddress];
            int attributeByte = _ppu.SpriteRam[spriteAddress + 2];
            int x = _ppu.SpriteRam[spriteAddress + 3];
            int tileIndex = _ppu.SpriteRam[spriteAddress + 1];

            int attrColor = ((attributeByte & 0x03) << 2) | 16;
            bool isInFront = (attributeByte & 32) == 32;
            bool flipX = (attributeByte & 64) == 64;
            bool flipY = (attributeByte & 128) == 128;

            int spritePatternTable = 0;
            // if these are 8x16 sprites, read high and low, draw
            if ((_ppu.PPUControlByte0 & 32) == 32)
            {
                if ((tileIndex & 1) == 1)
                {
                    spritePatternTable = 0x1000;
                }
                int[] getPatternTableEntry = GetSprite(spritePatternTable, tileIndex, attrColor, flipX, flipY);
                // spritePatternTable = spritePatternTable ^ 0x1000;
                tileIndex = tileIndex + 1;
                int[] getPatternTableEntryBottom = GetSprite(spritePatternTable, tileIndex, attrColor, flipX, flipY);

                if (flipY)
                {
                    MergeRect(getPatternTableEntryBottom, 8, 8, x - 1, y + 1, isInFront);
                    MergeRect(getPatternTableEntry, 8, 8, x - 1, y + 9, isInFront);
                }
                else
                {
                    MergeRect(getPatternTableEntry, 8, 8, x - 1, y + 1, isInFront);
                    MergeRect(getPatternTableEntryBottom, 8, 8, x - 1, y + 9, isInFront);
                }
            }
            else
            {
                // 8x8 sprites
                if ((_ppu.PPUControlByte0 & 0x08) == 0x08)
                {
                    spritePatternTable = 0x1000;
                }
                int[] getPatternTableEntry = GetSprite(spritePatternTable, tileIndex, attrColor, flipX, flipY);

                MergeRect(getPatternTableEntry, 8, 8, x - 1, y + 1, isInFront);
            }

            return;
        }

        public void DrawAllSprites()
        {
            for (int i = 63;i >= 0; --i)
            {

                DrawSprite(i);
            }
        }

        /// <summary>
        /// returns a 128x128 buffer for the tiles
        /// </summary>
        /// <param name="PatternTable"></param>
        /// <returns></returns>
        public int[] DoodlePatternTable(int PatternTable)
        {
            // return a 16x16 x 64 per tile pattern table for display
            int[] patterns = new int[0x4000];
            int[] tile;
            for (int j = 0; j < 16; ++j)
            {
                for (int i = 0; i < 16; ++i)
                {
                    tile = GetPatternTableEntry(PatternTable, (i) + j * 16, 0);
                    DrawTile(ref patterns, 128, 128, tile, i * 8, j * 8);
                }
            }
            return patterns;
        }

        /// <summary>
        /// returns a pixel array representing a current nametable in memory
        /// nametable will be 0,0x400, 0x800, 0xC00, mapped to 0x200 + Nametable
        /// </summary>
        /// <param name="NameTable"></param>
        /// <returns></returns>
        public int[] DoodleNameTable(int NameTable, MirrorMasks mirrorMask)
        {
            if (mirrorMask == MirrorMasks.OneScreenMask)
            {
                mirrorMask = (MirrorMasks)_ppu.CurrentMirrorMask;
            }
            int[] result = new int[256 * 240];

            for (int i = 0; i < 32; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    //int TileIndex = (byte)_ppu.NameTable[_ppu.Mirror[nt2], i + (j * 32)];
                    int address = 0x2000 + NameTable + i + (j * 32);
                    int TileIndex = (byte)_ppu.ChrRomHandler.GetPPUByte(0, address & (int)mirrorMask);

                    int addToCol = GetAttributeTableEntry(NameTable, i, j);
                    int[] tile = GetPatternTableEntry(_ppu.PatternTableIndex, TileIndex, addToCol);
                    DrawTile(ref result, 256, 240, tile, i * 8, j * 8);
                    //DrawRect(GetPatternTableEntry(_ppu.PatternTableIndex, TileIndex, addToCol), 8, 8, (i * 8) + XOffset, (j * 8) + YOffset);
                }
            }
            return result;
        }



        // width and height are of destBuffer, tile is assumed to be 8x8
        void DrawTile(ref int[] destBuffer, int width, int height, int[] tile, int xPos, int yPos)
        {

            for (int j = 0; j < 8; ++j)
            {
                for (int i = 0; i < 8; ++i)
                {

                    int xPosition = xPos + 8 - i;
                    int yPosition = (yPos + j) * width;

                    if (xPos > height) break;
                    if (yPosition + xPosition >= destBuffer.Length)
                        break;

                    destBuffer[yPosition + xPosition] = (byte)tile[(j * 8) + i];
                }
            }
        }



    }
}
