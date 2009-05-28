using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public unsafe partial class PixelWhizzler
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
        private NESSprite*[] currentSprites;

        private NESSprite[] unpackedSprites;
        
        private int _maxSpritesPerScanline = 8;

        public int MaxSpritesPerScanline
        {
            get { return _maxSpritesPerScanline; }
            set { _maxSpritesPerScanline = value; }
        }


        private byte[] spriteRAM = new byte[256];

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
            currentSprites = new NESSprite*[_maxSpritesPerScanline];
            for (int i = 0; i < _maxSpritesPerScanline; ++i)
            {
                currentSprites[i] = null;
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

            foreach(NESSprite* currSprite in currentSprites)
            {
                if (
                    (*currSprite).XPosition > 0
                    && currentXPosition >= (*currSprite).XPosition
                    && currentXPosition < (*currSprite).XPosition + 8)
                {

                    int spritePatternTable = 0;
                    if ((_PPUControlByte0 & 0x08) == 0x08)
                    {
                        spritePatternTable = 0x1000;
                    }
                    xPos = currentXPosition - (*currSprite).XPosition;
                    yLine = currentYPosition - (*currSprite).YPosition - 1;

                    yLine = yLine & (spriteSize - 1);

                    tileIndex = (*currSprite).TileIndex;

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
                        if ((*currSprite).SpriteNumber == 0)
                        {
                            spriteZeroHit = true;
                        }
                        isForegroundPixel = (*currSprite).Foreground;
                        return (byte)(result | (*currSprite).AttributeByte);
                    }
                }
            }
            return 0;
        }

        private byte WhissaSpritePixel(int patternTableIndex, int x, int y, NESSprite* sprite, int tileIndex)
        {
            // 8x8 tile
            int patternEntry;
            int patternEntryBit2;

            if ((*sprite).FlipY)
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
                ((*sprite).FlipX ?
                ((patternEntry >> x) & 0x1) | (((patternEntryBit2 >> x) << 1) & 0x2)
                : ((patternEntry >> 7 - x) & 0x1) | (((patternEntryBit2 >> 7 - x) << 1) & 0x2));
        }

        int spritesOnThisScanline;

        int spriteSize;
        /// <summary>
        /// populates the currentSpritesXXX arrays with the first 8 visible sprites on the 
        /// denoted scanline. 
        /// </summary>
        /// <param name="scanline">the scanline to preload sprites for</param>
        public void PreloadSprites(int scanline)
        {

            fixed (NESSprite* sprites = unpackedSprites)
            {
                NESSprite* currSprite = sprites;
                NESSprite* lastSprite = sprites + unpackedSprites.Length;

                spritesOnThisScanline = 0;
                while (currSprite < lastSprite)
                {
                    int y = (*currSprite).YPosition + 1;

                    if (scanline >= y && scanline < y + spriteSize)
                    {

                        currentSprites[spritesOnThisScanline] = currSprite;
                        (*currentSprites[spritesOnThisScanline]).IsVisible = true;

                        spritesOnThisScanline++;
                        if (spritesOnThisScanline == _maxSpritesPerScanline)
                        {
                            break;
                        }
                    }
                    currSprite++;
                }
                if (spritesOnThisScanline > 7)
                    _PPUStatus = _PPUStatus | 0x20;
            }
        }

        public void UnpackSprites()
        {
            fixed (NESSprite* sprites = unpackedSprites)
            {
                NESSprite* currSpritePtr = sprites;
                NESSprite* endSpritePtr = sprites + 64;
                int currSprite = 0;
                while (currSpritePtr < endSpritePtr)
                {
                    if (unpackedSprites[currSprite].Changed)
                    {
                        UnpackSprite(currSpritePtr, currSprite);
                    }
                    currSprite++;
                    currSpritePtr++;
                }
            }
        }

        private void UnpackSprite(NESSprite* sprite, int currSprite)
        {
            byte attrByte = spriteRAM[currSprite * 4 + 2];
            (*sprite).IsVisible = true;
            (*sprite).AttributeByte = ((attrByte & 0x03) << 2) | 0x10;
            (*sprite).YPosition = spriteRAM[currSprite * 4];
            (*sprite).XPosition = spriteRAM[currSprite * 4 + 3];
            (*sprite).SpriteNumber = currSprite;
            (*sprite).Foreground = (attrByte & 0x20) != 0x20;
            (*sprite).FlipX = (attrByte & 0x40) == 0x40;
            (*sprite).FlipY = (attrByte & 0x80) == 0x80;
            (*sprite).TileIndex = spriteRAM[currSprite * 4 + 1];
            (*sprite).Changed = false;
        }
    }
}
