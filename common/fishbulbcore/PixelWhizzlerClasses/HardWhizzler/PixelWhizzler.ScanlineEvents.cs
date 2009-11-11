using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class HardWhizzler
	{
        int latchedBGColor;

        bool NMIOccurred
        {
            get { return (_PPUStatus & 0x80) == 0x80; }
        }

        bool NMIHasBeenThrownThisFrame = false;

        public void SetupVINT()
        {
            _PPUStatus = _PPUStatus | 0x80;
            NMIHasBeenThrownThisFrame = false;
            // HandleVBlankIRQ = true;
            _frames = _frames + 1;
            //isRendering = false;
            needToDraw = false;

            if (NMIIsThrown)
            {
                nmiHandler();
                HandleVBlankIRQ = true;
                NMIHasBeenThrownThisFrame = true;
            }

        }

        public void ClearVINT()
        {
            _PPUStatus = 0;
            hitSprite = false;
            spriteSize = ((_PPUControlByte0 & 0x20) == 0x20) ? 16 : 8;
            //if ((_PPUControlByte1 & 0x18) != 0)
            //    isRendering = true;
            //scanlineNum = ScanlinePreRenderDummyScanline;
            //scanlinePos = 0;

            //RunNewScanlineEvents();

        }

        private void RunEndOfScanlineRenderEvents()
        {

        }

        private void RunNewScanlineEvents()
        {

            if (scanlineNum == ScanlineRenderingStartsOn)
            {
                vbufLocation = 0;
            }

            if (scanlineNum >= ScanlineRenderingStartsOn && scanlineNum <= ScanlineRenderingEndsOn)
            {
                lockedHScroll = _hScroll;
                // nameTableMemoryStart = (0x400 * (_PPUControlByte0 & 0x3)); 
                PreloadSprites(scanlineNum - ScanlineRenderingStartsOn);
                if (spritesOnThisScanline >= 7)
                {
                    _PPUStatus = _PPUStatus | 0x20;
                }
                // lock hscroll at the beginning of each scanline 
                // (TODO: actually, every 4 clocks on a scanline?)
                //UpdateMirroring();
                if (HBlank != null) HBlank(this, new EventArgs());

            }


        }

        private void UpdateSprites()
        { 
            // sprite enable
            // left col object clipping
            // active object pattern table
            // color bits
            // b/w color
        }

        private void UpdateTiles()
        {
            // color bits
            // b/w color
            // background enable
            // left col bg clipping
            // scroll regs
            // x/y nametable 
            // pattern table
        }
	}
}
