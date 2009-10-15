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
//                    FillBuffer();
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

                    //curBufPos++;
                    vbufLocation++;
                    //					nextPixel++;
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

        public void FillBuffer()
        {

            if (fillRGB)
            {
                int i = 0;
                while (i < 256 * 256 -1)
                {
                    rgb32OutBuffer[i] = pal[_palette[rgb32OutBuffer[i]] & 0xFF];
                    i++;
                }
            }
            //else
            //{
            //    fixed (byte* pal = _palette)
            //    {
            //        byte* palPtr = pal;
            //        outBufPtr += 255 * 256;
            //        for (int i = 0; i < 32; ++i)
            //        {
            //            *outBufPtr++ = *palPtr++;
            //        }
            //    }

            //}

        }

        public int[] VideoBuffer
        {
            get
            {
                return rgb32OutBuffer;
                // return rgb32OutBuffer; 
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
            int tilePixel = _tilesAreVisible ? GetNameTablePixel() : 0;
            isForegroundPixel = false;
            int spritePixel =  _spritesAreVisible ? GetSpritePixel() : 0;


            if (!hitSprite && spriteZeroHit && tilePixel !=0 )
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            if (fillRGB)
            {
                //rgb32OutBuffer[i] = pal[_palette[rgb32OutBuffer[i]] & 0xFF];
                rgb32OutBuffer[vbufLocation] = (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                    ? pal[_palette[spritePixel]] : pal[_palette[tilePixel]];
            }
            else
            {
                rgb32OutBuffer[vbufLocation] =
                    (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                    ? spritePixel : tilePixel;
            }
        }

        private void DrawClipPixel()
        {
            int tilePixel = 0;


            // if we're clipping the left 8 pixels, or bg is not visible, set color to background byte
            if (_tilesAreVisible && !ClippingTilePixels())
            {
                tilePixel = GetNameTablePixel();
            }
            isForegroundPixel = false;
            int spritePixel = _spritesAreVisible && !ClippingSpritePixels() ? GetSpritePixel() : 0;


            //&& (newbyte & 3) != 0
            if (!hitSprite && spriteZeroHit)
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            if (fillRGB)
            {
                rgb32OutBuffer[vbufLocation] = (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                    ? pal[_palette[spritePixel]] : pal[_palette[tilePixel]];
            }
            else
            {
                rgb32OutBuffer[vbufLocation] =
                    (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                    ? spritePixel : tilePixel;
            }
        }


        private bool _clipTiles;
        private bool _clipSprites;

        private bool ClippingTilePixels()
        {
            return _clipTiles && currentXPosition < 8;
        }

        private bool ClippingSpritePixels()
        {
            return _clipSprites && currentXPosition < 8;
        }
    }
}
