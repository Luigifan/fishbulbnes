using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler : NES.CPU.PixelWhizzlerClasses.IPPU
    {

//0..19:	Starting at the instant the VINT flag is pulled down (when a NMI is 
//generated), 20 scanlines make up the period of time on the PPU which I like 
//to call the VINT period. During this time, the PPU makes no access to it's 
//external memory (i.e. name / pattern tables, etc.).

//20:	After 20 scanlines worth of time go by (since the VINT flag was set), 
//the PPU starts to render scanlines. This first scanline is a dummy one; 
//although it will access it's external memory in the same sequence it would 
//for drawing a valid scanline, no on-screen pixels are rendered during this 
//time, making the fetched background data immaterial. Both horizontal *and* 
//vertical scroll counters are updated (presumably) at cc offset 256 in this 
//scanline. Other than that, the operation of this scanline is identical to 
//any other. The primary reason this scanline exists is to start the object 
//render pipeline, since it takes 256 cc's worth of time to determine which 
//objects are in range or not for any particular scanline.

//21..260: after rendering 1 dummy scanline, the PPU starts to render the 
//actual data to be displayed on the screen. This is done for 240 scanlines, 
//of course.

//261:	after the very last rendered scanline finishes, the PPU does nothing 
//for 1 scanline (i.e. the programmer gets screwed out of perfectly good VINT 
//time). When this scanline finishes, the VINT flag is set, and the process of 
//drawing lines starts all over again.


        // 89342 ppu clocks per frame
        // 262 scanlines * 341 clocks per scanline
        // about 20 scanlines of vblank = 6820
        // 240 lines rendered
        const int ScanlinePreRenderDummyScanline = 20;
        const int ScanlineRenderingStartsOn = 21;
        const int ScanlineRenderingEndsOn = 260;

        // vblank thrown at end of this line (dummy line before vblank thrown, not drawn)
        
        // this resets counter to 0 (is not run)

        // length of a scanline, rendered stays 256.  
        //    increasing total length from 340 overclocks the cpu

        const int ScanlineLastRenderedPixel = 255;
        const int ScanlineTotalLength = 340;
        //const int ScanlineTotalLength = 340;

        const int ScanlineEventPPUXIncremented = 3;
        const int ScanlineEventPPUXReset = 257;
        const int ScanlineEventPPUYIncremented = 251;

        int currentXPosition = 0, currentYPosition = 0;
        const int vBufferWidth = 0x100;

        public int CurrentYPosition
        {
            get { return currentYPosition; }
        }

        public int CurrentXPosition
        {
            get { return currentXPosition; }
        }

        int scanlineNum = 0, scanlinePos = 0;


        public int ScanlinePos
        {
            get { return scanlinePos; }
        }

        public int ScanlineNum
        {
            get { return scanlineNum; }
        }


        static PixelWhizzler()
        {
            for (int i = 0; i < 32; ++i)
            {
                _powersOfTwo[i] = (int)Math.Pow(2.0, (double)i);
            }
        }

        private static int[] _powersOfTwo = new int[32];

        public static int[] PowersOfTwo
        {
            get
            {
                return _powersOfTwo;
            }
        }

    }
}
