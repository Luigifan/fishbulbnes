using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo.Interaction;
using NES.CPU.Machine;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {

        protected int lastcpuClock;

        public int LastcpuClock
        {
            get { return lastcpuClock; }
            set { lastcpuClock = value; }
        }
        /// <summary>
        /// draws from the lastcpuClock to the current one
        /// </summary>
        /// <param name="cpuClockNum"></param>
        public virtual void DrawTo(int cpuClockNum)
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

        protected virtual void BumpScanline()
        {
            switch (frameClock++)
            {
                case 0:
                    //frameFinished();
                    break;
                case 6820:
                    ClearVINT();

                    frameOn = true;
                    //
                    ClearNESPalette();
                    chrRomHandler.ResetBankStartCache();
                    // setFrameOn();
                    if (spriteChanges)
                    {
                        UnpackSprites();
                        spriteChanges = false;
                    }


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
                    shouldRender = true;
                    if (frameFinished != null)
                        frameFinished();
                    SetupVINT();
                    frameOn = false;
                    frameClock = 0;

                    if (_isDebugging)
                        events.Clear();

                    break;
            }



            if (frameClock >= 7161 && frameClock <= 89342)
            {


                if (currentXPosition < 256 && vbufLocation < 256 * 240)
                {
                    UpdateXPosition();
                    DrawPixel();

                    vbufLocation++;
                }
                if (currentXPosition == 256)
                {
                    chrRomHandler.UpdateScanlineCounter();
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

                    UpdatePixelInfo();
                    RunNewScanlineEvents();

                }

            }


        }

        protected virtual void UpdateXPosition()
        {
            xPosition = currentXPosition + lockedHScroll;


            if ((xPosition & 7) == 0)
            {
                xNTXor = ((xPosition & 0x100) == 0x100) ? 0x400 : 0x00;


                xPosition &= 0xFF;

                FetchNextTile();
            }
        }

        protected int[] rgb32OutBuffer = new int[256*256];

        protected int[] outBuffer = new int[256 * 256];

        public int[] OutBuffer
        {
            get { return outBuffer; }
        }

        private int[] pixelEffectBuffer = new int[256 * 256];

        public int[] PixelEffectBuffer
        {
            get { return pixelEffectBuffer; }
            set { pixelEffectBuffer = value; }
        }


        public virtual void FillBuffer()
        {

            int i = 0;
            while (i < 256 * 240 )
            {
                int tile = (outBuffer[i] & 0x0F);
                int sprite = ((outBuffer[i] >> 4) & 0x0F) + 16;
                int isSprite = (outBuffer[i] >> 8) & 64;
                int curPal = (outBuffer[i] >> 24) & 0xFF;

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

        protected int vbufLocation;

        protected int pixelWidth = 32;

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

        protected virtual void DrawPixel() {}

        public virtual void UpdatePixelInfo()
        {
            nameTableMemoryStart = nameTableBits * 0x400;
        }

        int[] drawInfo= new int[256*256];


        IPixelAwareDevice pixelDevices = null;

        public IPixelAwareDevice PixelAwareDevice
        {
            get { return pixelDevices; }
            set { pixelDevices = value; }
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
