using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NES.CPU.Fastendo;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler : IPPU
    {




//+----------------+
//|2C02 programming|
//+----------------+
//This section lays out how 2C02 ports & programmable internal memory 
//structures are organized. Names for these ports throughout the document will 
//simply consist of adding $200 to the end of the number (i.e., $2002). 
//Anything not explained here will be later on.




//Readable 2C02 registers
//-----------------------
//reg	bit	desc
//---	---	----
//2	5	more than 8 objects on a single scanline have been detected in the last 
//frame
//    6	a primary object pixel has collided with a playfield pixel in the last 
//frame
//    7	vblank flag

//4	-	object attribute memory write port (incrementing port 3 thenafter)

//7	-	PPU memory read port.


//Object attribute structure (4*8 bits)
//-------------------------------------
//ofs	bit	desc
//---	---	----
//0	-	scanline coordinate minus one of object's top pixel row.

//1	-	tile index number. Bit 0 here controls pattern table selection when reg 
//0.5 = 1.

//2	0	palette select low bit
//    1	palette select high bit
//    5	object priority (> playfield's if 0; < playfield's if 1)
//    6	apply bit reversal to fetched object pattern table data
//    7	invert the 3/4-bit (8/16 scanlines/object mode) scanline address used to 
//access an object tile

//3	-	scanline pixel coordite of most left-hand side of object.

        public event EventHandler HBlank;


        private bool shouldRender = false;

        public bool ShouldRender
        {
            get { return shouldRender; }
            set { shouldRender = value; }
        }

        private byte[] vBuffer;


        private int nameTableIndex;
        private int _frames = 0;


        public int Frames
        {
            get { return _frames; }
        }

        private byte[] _vidRAM = new byte[0x4000];

        public byte[] VidRAM
        {
            get { return _vidRAM; }
            set { _vidRAM = value; }
        }

        public byte[] cartCopyVidRAM
        {
            get { return _vidRAM; }
            set { _vidRAM = value; }
        }

        public int VidRAM_GetNTByte(int address)
        {
            int result = 0;
            if (address >= 0x2000 && address < 0x3000)
            {

                result = _vidRAM[(address & (int)currentMirrorMask) | oneScreenMirrorOffset];

            }
            else 
            {
                result = _vidRAM[address];
            }
            return result;
        }

        private bool hitSprite = false;

        public bool HandleVBlankIRQ
        {
            get;
            set;
        }


        private int[] VROM
        {
            get;
            set;
        }

        private int _PPUControlByte0;
        
        private int _PPUControlByte1;

        #region control byte 0 members

        public int PPUControlByte0
        {
            get { return _PPUControlByte0; }
            set { 
                if (_PPUControlByte0 != value)
                {
                    _PPUControlByte0 = value;
                    UpdatePPUControlByte0();
                }
            }
        }

        private void UpdatePPUControlByte0()
        {
            if ((_PPUControlByte0 & 0x10) == 0x10)
                _backgroundPatternTableIndex = 0x1000;
            else
                _backgroundPatternTableIndex = 0;
        }

        public bool NMIIsThrown
        {
            get { return (_PPUControlByte0 & 0x80) == 0x80; }
        }

        #endregion

        #region control byte 1 members

        public int PPUControlByte1
        {
            get { return _PPUControlByte1; }
            set { _PPUControlByte1 = value; }
        }

        public bool BackgroundVisible
        {
            get { return _tilesAreVisible; }
        }


        private bool _spritesAreVisible;
        private bool _tilesAreVisible;

        public bool SpritesAreVisible
        {
            get { return _spritesAreVisible; }
        }

        #endregion


        private int _PPUStatus;

        public int PPUStatus
        {
            get { return _PPUStatus; }
            set { _PPUStatus = value; }
        }

        private int _PPUAddress;

        public int PPUAddress
        {
            get { return _PPUAddress; }
            set { _PPUAddress = value; }
        }


        public PixelWhizzler()
        {
            InitSprites();

            vBuffer = new byte[240 * 256];

            LoadPalABGR();
        }

        private int ppuReadBuffer;


        private bool PPUAddressLatchIsHigh = true;

        public void LoadPalABGR()
        {
        //Open App.Path & "\" + file For Binary As #FileNum

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NES.CPU.bnes.pal") )
            {
                for (int n = 0 ; n < 64; ++n)
                {
                    int r = stream.ReadByte();
                    int g = stream.ReadByte();
                    int b = stream.ReadByte();
                    pal[n] = (0xFF <<24) | (r << 16) | (g << 8) | b;
                    pal[n + 64] = pal[n];
                    pal[n + 128] = pal[n];
                    pal[n + 192] = pal[n];
                }
            }
        }

        public static int[] GetPalABGR()
        {
            //Open App.Path & "\" + file For Binary As #FileNum
            int[] tPal = new int[256];
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NES.CPU.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    int r = stream.ReadByte();
                    int g = stream.ReadByte();
                    int b = stream.ReadByte();
                    tPal[n] = (0xFF << 24) | (r << 16) | (g << 8) | b;
                    tPal[n + 64] = pal[n];
                    tPal[n + 128] = pal[n];
                    tPal[n + 192] = pal[n];
                }
            }
            return tPal;
        }

        public static int[] GetPalRGBA()
        {
            //Open App.Path & "\" + file For Binary As #FileNum
            int[] tPal = new int[256];
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NES.CPU.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    tPal[n] = (b << 16) | (g << 8) | (r << 0);
                    tPal[n + 64] = pal[n];
                    tPal[n + 128] = pal[n];
                    tPal[n + 192] = pal[n];
                }
            }
            return tPal;
        }


        /// <summary>
        /// Initializes the rendering pallette with the bytes in a BGR format, instead of the default RGB format
        /// </summary>
        public void LoadPalRGBA()
        {
            //Open App.Path & "\" + file For Binary As #FileNum

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NES.CPU.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    pal[n] = (b << 16) | (g << 8) | (r << 0);
                    pal[n + 64] = pal[n];
                    pal[n + 128] = pal[n];
                    pal[n + 192] = pal[n];
                }
            }
        }


        int[] p32 = new int[256];
        public static readonly int[] pal = new int[256];

        /// <summary>
        /// Fills an external buffer with rgb color values, relative to current state of PPU's pallete ram
        /// </summary>
        /// <param name="buffer"></param>
        public void SetupBufferForDisplay(ref int[] buffer)
        {
            for (int i = 0; i < 30; ++i)
            {
                p32[i] = pal[i]; // pal[_vidRAM[i + 0x3F00]];
            }

            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = p32[buffer[i]];
                buffer[i + 1] = p32[buffer[i + 1] & 0xFF];
                buffer[i + 2] = p32[buffer[i + 2] & 0xFF];
                buffer[i + 3] = p32[buffer[i + 3] & 0xFF];
            }
        }

        int _backgroundPatternTableIndex;
        public int PatternTableIndex 
        {
            get
            {
                return _backgroundPatternTableIndex; 
            }
        }

        #region IMemoryMappable Members

        bool needToDraw = true;

        public bool NeedToDraw
        {
            get { return needToDraw; }
        }


        #endregion

        #region IPPU Members



        private bool isRendering =true;

        public bool IsRendering
        {
            get { return isRendering; }
        }

        // i have lines 0-20 as vblank, 21-260 rendered scanlines, 
        //261 is the do-nothing scanline when vblank is thrown and its time to draw.

        // note, lots of others document this as 0-239 are rendred, dummy, 241-261 vblank
        //  it seems to me it's 6 of one half dozen of the other, the numbering is abitrary

        public int frameClock = 0;
        public const int frameClockEnd = 89342;
        public bool FrameEnded = false;

        private bool frameOn = false;

        public bool FrameOn
        {
            get { return frameOn; }
            set { frameOn = value; }
        }


        int[] framePalette = new int[0x100];

        private int nameTableMemoryStart;

        public int NameTableMemoryStart
        {
            get { return nameTableMemoryStart; }
            set { nameTableMemoryStart = value;
            }
        }

        public byte[] CurrentFrame
        {
            get { return vBuffer; }
        }

        public void RenderScanline(int scanlineNum)
        {
            throw new NotImplementedException();
        }


        #endregion








    }
}
