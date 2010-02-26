using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
	public partial class PixelWhizzler
	{
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

        protected virtual void RunNewScanlineEvents()
        {




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
