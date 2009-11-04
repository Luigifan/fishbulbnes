using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo.Interaction;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {

        private int lastcpuClock;

        public int LastcpuClock
        {
            get { return lastcpuClock; }
            set { lastcpuClock = value; }
        }
        /// <summary>
        /// draws from the lastcpuClock to the current one
        /// </summary>
        /// <param name="cpuClockNum"></param>
        public void DrawTo(int cpuClockNum)
        {
            int frClock = (cpuClockNum - lastcpuClock )* 3;
            
            //// if we are in vblank 
            if (frameClock < 6820)
            {
                // if the frameclock +frClock is in vblank (< 6820) dont do nothing, just update it
                if (frameClock + frClock < 6820)
                {
                    frameClock += frClock;
                    frClock = 0;
                }
                else
                {
                    frClock += frameClock - 6820;
                    frameClock = 6820;
                }
            }
            for (int i = 0; i < frClock; ++i)
            {
                BumpScanline();
            }
            lastcpuClock = cpuClockNum;
        }

        private void BumpScanline()
        {
            switch (frameClock++)
            {
                case 0:
                    frameFinished();
                    break;
                case 6820:
                    frameOn = true;
                    //
                    currentPalette = 0;
                    palCache[currentPalette] = new byte[32];
                    Buffer.BlockCopy(_palette, 0, palCache[currentPalette], 0, 32);
                    // setFrameOn();
                    if (spriteChanges)
                    {
                        UnpackSprites();
                        spriteChanges = false;
                    }

                    ClearVINT();
                    break;
                //304 pixels into pre-render scanline
                case 7125:
                    break;

                case 7161:
                    //lockedVScroll = _vScroll;
                    vbufLocation = 0;
                    //curBufPos = bufStart;

                    xNTXor = 0x0;
                    yNTXor = 0;
                    currentXPosition = 0;
                    currentYPosition = 0;

                    break;

                case frameClockEnd:
                    if (fillRGB) FillBuffer();
                    
                    SetupVINT();
                    frameFinished();
                    frameOn = false;

                    frameClock = 0;
                    break;
            }



            if (frameClock >= 7161 && frameClock <= 89342)
            {


                if (currentXPosition < 256 && vbufLocation < 256 * 240)
                {

                    xPosition = currentXPosition + lockedHScroll;
                    if ((xPosition & 7) == 0)
                    {
                        xNTXor = ((xPosition & 0x100) == 0x100) ? 0x400 : 0x00;
                        xPosition &= 0xFF;

                        FetchNextTile();
                    }

                    if (currentXPosition < 8)
                        DrawClipPixel();
                    else
                        DrawPixel();

                    vbufLocation++;
                }

                currentXPosition++;

                if (currentXPosition > 340)
                {
                    currentXPosition = 0;
                    currentYPosition++;

                    PreloadSprites(currentYPosition);
                    if (spritesOnThisScanline >= 7)
                    {
                        _PPUStatus = _PPUStatus | 0x20;
                    }

                    lockedHScroll = _hScroll;

                    yPosition = currentYPosition + lockedVScroll;

                    if (yPosition < 0)
                    {
                        yPosition += 240;
                    }
                    if (yPosition >= 240)
                    {
                        yPosition -= 240;
                        yNTXor = 0x800;
                    }
                    else
                    {
                        yNTXor = 0x00;
                    }

                }

            }


        }

        private int[] rgb32OutBuffer = new int[256*256];

        private uint[] outBuffer = new uint[256 * 256];

        public uint[] OutBuffer
        {
            get { return outBuffer; }
        }

        private int[] pixelEffectBuffer = new int[256 * 256];

        public int[] PixelEffectBuffer
        {
            get { return pixelEffectBuffer; }
            set { pixelEffectBuffer = value; }
        }


        public void FillBuffer()
        {

            int i = 0;
            while (i < 256 * 240 )
            {
                uint tile = (outBuffer[i] & 0xFF);
                uint sprite = (outBuffer[i] >> 8) & 0xFF;
                uint isSprite = (outBuffer[i] >> 16) & 0xFF;
                uint curPal = (outBuffer[i] >> 24) & 0xFF;

                uint pixel;
                if (isSprite > 0)
                {
                    pixel = palCache[curPal][sprite];
                }
                else
                {
                    pixel = palCache[curPal][tile];
                }
                rgb32OutBuffer[i] = pal[pixel];
                i++;
            }
        }

        public int[] VideoBuffer
        {
            get
            {
                return rgb32OutBuffer;
            }
        }

        public void SetVideoBuffer(int[] inBuffer)
        {
            rgb32OutBuffer = inBuffer;
        }

        bool frameEnded = false;

        /// <summary>
        /// Checks if NMI needs to be reasserted during vblank
        /// </summary>
        public void CheckVBlank()
        {
            if (!NMIHasBeenThrownThisFrame && !frameOn && NMIIsThrown && NMIOccurred )
            {
                nmiHandler();
                HandleVBlankIRQ = true;
                NMIHasBeenThrownThisFrame = true;
            }
        }

        int vbufLocation;

        private int pixelWidth = 32;

        public int PixelWidth
        {
            get { return pixelWidth; }
            set { pixelWidth = value; }
        }

        bool fillRGB = false;

        public bool FillRGB
        {
            get { return fillRGB; }
            set { fillRGB = value; }
        }

        private void DrawPixel()
        {
            byte tilePixel = _tilesAreVisible ? GetNameTablePixel() :(byte)0;
            isForegroundPixel = false;
            byte spritePixel = _spritesAreVisible ? GetSpritePixel() : (byte)0;

            if (!hitSprite && spriteZeroHit && tilePixel !=0 )
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            // the int is packed like this:
            //  byte 0 : the current tile pixel
            //  byte 1 : the current sprite pixel
            //  byte 2 : bit 0 : is foreground sprite
            //           bit 1 : tiles visible
            //           bit 2 : sprites visible
            //  byte 4 : current palette  (future project)
            outBuffer[vbufLocation] =  (uint)(
                currentPalette << 24 |
                ((spritePixel != 0 && (tilePixel == 0 || isForegroundPixel)) ? 255 : 0) << 16 |
                spritePixel << 8 | 
                tilePixel);

            pixelEffectBuffer[vbufLocation] = currentTileIndex;

            //(spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                //? _palette[spritePixel] : _palette[tilePixel];
        }

        private void DrawClipPixel()
        {
            int tilePixel = 0;


            // if we're clipping the left 8 pixels, or bg is not visible, set color to background byte
            if (_tilesAreVisible && !_clipTiles)
            {
                tilePixel = GetNameTablePixel();
            }
            isForegroundPixel = false;
            int spritePixel = _spritesAreVisible && !_clipSprites ? GetSpritePixel() : 0;


            //&& (newbyte & 3) != 0
            if (!hitSprite && spriteZeroHit)
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            outBuffer[vbufLocation] = (uint)(
                currentPalette << 24 |
                ((spritePixel != 0 && (tilePixel == 0 || isForegroundPixel)) ? 255 : 0) << 16 |
                spritePixel << 8 |
                tilePixel);

            //outBuffer[vbufLocation] =
            //    (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
            //    ? _palette[spritePixel] : _palette[tilePixel];
        }


        private bool _clipTiles;
        private bool _clipSprites;

        private bool ClippingTilePixels()
        {
            return _clipTiles ;
        }

        private bool ClippingSpritePixels()
        {
            return _clipSprites ;
        }
    }
}
