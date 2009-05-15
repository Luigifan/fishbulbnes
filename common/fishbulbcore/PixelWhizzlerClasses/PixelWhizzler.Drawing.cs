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
            int spritePixel = _spritesAreVisible ? GetSpritePixel() : 0;

            if (!hitSprite && spriteZeroHit && tilePixel !=0 )
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            if (fillRGB)
            {
                rgb32OutBuffer[vbufLocation] = (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                    ? pal[rgb32OutBuffer[255 * 256 + spritePixel]] : pal[rgb32OutBuffer[255 * 256 + tilePixel]];
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
            int spritePixel = 0;

            // if we're clipping the left 8 pixels, or bg is not visible, set color to background byte
            if (_tilesAreVisible && !ClippingTilePixels())
            {
                tilePixel = GetNameTablePixel();
            }
            isForegroundPixel = false;
            if (_spritesAreVisible && !ClippingSpritePixels())
            {
                spritePixel = GetSpritePixel();
            }
            //&& (newbyte & 3) != 0
            if (!hitSprite && spriteZeroHit)
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            if (fillRGB)
            {
                rgb32OutBuffer[vbufLocation] = (spritePixel != 0 && (tilePixel == 0 || isForegroundPixel))
                    ? pal[rgb32OutBuffer[255*256 + spritePixel]] : pal[rgb32OutBuffer[255*256+tilePixel]];
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
