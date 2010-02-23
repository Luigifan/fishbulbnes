using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler : IClockedMemoryMappedIOElement
    {

        protected int nameTableBits = 0; 
        private bool vidRamIsRam = true;

        protected byte[] _palette = new byte[0x20];

        public byte[] Palette
        {
            get { return _palette; }
            set { _palette = value; }
        }

        public void SetByte(int Clock, int address, int data)
        {
            DrawTo(Clock);

            if (_isDebugging)
            {
                Events.Enqueue(new PPUWriteEvent { DataWritten = data, FrameClock = frameClock, RegisterAffected = address, ScanlineNum = frameClock / 341, ScanlinePos = frameClock % 341 });
            }
            needToDraw = true;
            //Writable 2C02 registers
            //-----------------------

            //4 	-	returns object attribute memory 
            //      location indexed by port 3, then increments port 3.

            //6	    -	PPU address port to access with port 7.

            //7	    -	PPU memory write port.


            switch (address & 0x7)
            {
                //reg	bit	desc
                //---	---	----
                //0	    0	X scroll name table selection.
                //      1	Y scroll name table selection.
                //      2	increment PPU address by 1/32 (0/1) on access to port 7
                //      3	object pattern table selection (if bit 5 = 0)
                //      4	playfield pattern table selection
                //      5	8/16 scanline objects (0/1)
                //      6	EXT bus direction (0:input; 1:output)
                //      7	/VBL disable (when 0)
                case 0x0:
                    _PPUControlByte0 = data;

                    nameTableMemoryStart = (0x400 * (_PPUControlByte0 & 0x3));
                    nameTableBits = (byte)(_PPUControlByte0 & 0x3);
                    _backgroundPatternTableIndex = ((_PPUControlByte0 & 0x10) >> 4) * 0x1000;

                    // if we toggle /vbl we can throw multiple NMIs in a vblank period
                    if ((data & 0x80) == 0x80 && NMIHasBeenThrownThisFrame)
                    {
                        // NMIHasBeenThrownThisFrame = false;
                    }
                    break;
                case 0x1:
                    //1	    0	disable composite colorburst (when 1). Effectively causes gfx to go black & white.
                    //      1	left side screen column (8 pixels wide) playfield clipping (when 0).
                    //      2	left side screen column (8 pixels wide) object clipping (when 0).
                    //      3	enable playfield display (on 1).
                    //      4	enable objects display (on 1).
                    //      5	R (to be documented)
                    //      6	G (to be documented)
                    //      7	B (to be documented)

                    isRendering = (data & 0x18)!=0;
                    _PPUControlByte1 = data;
                    
                    _spritesAreVisible = (_PPUControlByte1 & 0x10) == 0x10;
                    _tilesAreVisible = (_PPUControlByte1 & 0x08) == 0x08;
                    _clipTiles = (_PPUControlByte1 & 0x2) != 0x2;
                    _clipSprites = (_PPUControlByte1 & 0x4) != 0x4;

                    break;
                case 0x2:
                    ppuReadBuffer = data;
                    break;
                case 0x3:
                    //3	    -	internal object attribute memory index pointer 
                    //          (64 attributes, 32 bits each, byte granular access). 
                    //          stored value post-increments on access to port 4.
                    _spriteAddress = data & 0xFF;
                    break;
                case 0x4:
                    spriteRAM[_spriteAddress] = (byte)data;
                    // UnpackSprite(_spriteAddress / 4);
                    _spriteAddress = (_spriteAddress + 1) & 0xFF;
                    unpackedSprites[_spriteAddress / 4].Changed = true;
                    spriteChanges = true;
                    break;
                case 0x5:

                    //5	    -	scroll offset port.
                    // on 1st read (high), bits 0,1,2 go to fine horizonal scroll, rest to select tile
                    // on 2nd read, bits 0,1,2 go to fine vertical scroll, rest to select tile
                    // during render, writes to FH are applied immediately
                    if (PPUAddressLatchIsHigh)
                    {
                        //if (isRendering)
                        //{
                        //    fineHorizontalScroll = data & 0x7;
                        //    horizontalTileIndex = data >> 3;
                        //}  
                        _hScroll = data;
                        lockedHScroll = _hScroll & 0x7;
                        PPUAddressLatchIsHigh = false;
                    }
                    else
                    {
                        // during rendering, a write here will not post to the rendering counter

                        _vScroll = data;
                        if (data > 240)
                        {
                            _vScroll = data - 256;
                        }
                        //note: i shouldnt need this once the scanlineevent timing is fixed
                        if (!frameOn || (frameOn && !isRendering))
                        {
                            lockedVScroll = _vScroll;
                            UpdatePixelInfo();
                        }
                        
                        // fineVerticalScroll = data & 0x7;
                        // verticalTileIndex = data >> 3;
                        PPUAddressLatchIsHigh = true;
                    }
                    break;

                case 0x6:

                    //Since the PPU's external address bus is only 14 bits in width, 
                    //the top two bits of the value written are ignored. 
                    if (PPUAddressLatchIsHigh)
                    {
                        //            //a) Write upper address byte into $2006
                        _PPUAddress = (_PPUAddress & 0xFF) | ((data & 0x3F) << 8);
                        PPUAddressLatchIsHigh = false;
                    }
                    else
                    {
                        //            //b) Write lower address byte into $2006
                        _PPUAddress = (_PPUAddress & 0x7F00) | data & 0xFF;
                        PPUAddressLatchIsHigh = true;

                        // writes here during rendering directly affect the scroll counter
                        // from Marat Fazulamans doc

                        //Address Written into $2006
                        //xxYYSSYYYYYXXXXX
                        //   | |  |     |
                        //   | |  |     +---- Horizontal scroll in tiles (i.e. 1 = 8 pixels)
                        //   | |  +--------- Vertical scroll in tiles (i.e. 1 = 8 pixels)
                        //   | +------------ Number of Name Table ($2000,$2400,$2800,$2C00)
                        //   +-------------- Additional vertical scroll in pixels (0..3)
                        
                        // on second write during frame, loopy t (_hscroll, _vscroll) is copied to loopy_v (lockedHscroll, lockedVScroll)
                        _hScroll = ((_PPUAddress & 0x1F) << 3); // +(currentXPosition & 7);
                        _vScroll = (((_PPUAddress >> 5) & 0x1F) << 3);
                        _vScroll |= ((_PPUAddress >> 12) & 0x3);

                        nameTableBits = ((_PPUAddress >> 10) & 0x3);
                        nameTableMemoryStart = ((_PPUAddress >> 10) & 0x3) * 0x400;
                        if (frameOn)
                        {
                            lockedHScroll = _hScroll;
                            lockedVScroll = _vScroll;
                            lockedVScroll -= currentYPosition;
                        }

                        // relock vscroll during render when this happens
                    }


                    break;
                case 0x07:
                    //            //Writing to PPU memory:
                    //            //c) Write data into $2007. After each write, the
                    //            //   address will increment either by 1 (bit 2 of
                    //            //   $2000 is 0) or by 32 (bit 2 of $2000 is 1).
                    
                    // ppuLatch = data;
                    if ((_PPUAddress & 0xFF00) == 0x3F00)
                    {

                        WriteToNESPalette(_PPUAddress, (byte)data);
                        // these palettes are all mirrored every 0x10 bytes


                        // _vidRAM[_PPUAddress ^ 0x1000] = (byte)data;
                    }
                    else
                    {
                        // if its a nametable byte, mask it according to current mirroring
                        if ((_PPUAddress & 0xF000) == 0x2000)
                        {
                            chrRomHandler.SetPPUByte(Clock,_PPUAddress ,(byte)data);
                        }
                        else
                        {
                            if (vidRamIsRam)
                            {
                                chrRomHandler.SetPPUByte(Clock, _PPUAddress,  (byte)data);
                            }
                        }
                    }
                    // if controlbyte0.4, set ppuaddress + 32, else inc
                    if ((PPUControlByte0 & 0x4) == 0x4)
                    {
                        _PPUAddress = (_PPUAddress + 32);
                    }
                    else
                    {
                        _PPUAddress = (_PPUAddress + 1);
                    }
                    // reset the flag which makex xxx6 set the high byte of address

                    PPUAddressLatchIsHigh = true;
                    PPUAddress = (PPUAddress & 0x3FFF);
                    break;
            }
            UpdatePixelInfo();
        }

        public int GetByte(int Clock, int address)
        {

            switch (address & 0x7)
            {
                case 0x0:
                case 0x1:
                case 0x5:
                case 0x6:
                    return ppuReadBuffer;
                case 0x2:
                    DrawTo(Clock);

                    int ret;
                    PPUAddressLatchIsHigh = true;
                    // bit 7 is set to 0 after a read occurs
                    // return lower 5 latched bits, and the status
                    ret = (ppuReadBuffer & 0x1F) | _PPUStatus;
                    //ret = _PPUStatus;
                    
                    //{
                    //If read during HBlank and Bit #7 of $2000 is set to 0, then switch to Name Table #0
                    //if ((PPUControlByte0 & 0x80) == 0 && scanlinePos > 0xFF)
                    //{
                    //    nameTableMemoryStart = 0;
                    //}
                    // clear vblank flag if read
                    if ((ret & 0x80) == 0x80)
                        _PPUStatus = _PPUStatus & ~0x80;

                    //}
                    return ret;
                case 0x4:
                    int tmp = spriteRAM[_spriteAddress];
                    //ppuLatch = spriteRAM[SpriteAddress];
                    // should not increment on read ?
                    //SpriteAddress = (SpriteAddress + 1) & 0xFF;
                    return tmp;
                case 0x7:
                    //        If Mapper = 9 Then
                    //            If PPUAddress < &H2000& Then
                    //                map9_latch tmp, (PPUAddress And &H1000&)
                    //            End If
                    //        End If
                    // palette reads shouldn't be buffered like regular vram reads, they re internal
                    if ((PPUAddress & 0xFF00) == 0x3F00)
                    {
                        // these palettes are all mirrored every 0x10 bytes
                        tmp = _palette[PPUAddress & 0x1F];
                        // palette read should also read vram into read buffer

                        // info i found on the nesdev forums

                        // When you read PPU $3F00-$3FFF, you get immediate data from Palette RAM 
                        // (without the 1-read delay usually present when reading from VRAM) and the PPU 
                        // will also fetch nametable data from the corresponding address (which is mirrored from PPU $2F00-$2FFF). 

                        // note: writes do not work this way 
                        ppuReadBuffer = chrRomHandler.GetPPUByte(Clock, _PPUAddress - 0x1000);
                    }
                    else
                    {
                        tmp = ppuReadBuffer;
                        if (_PPUAddress >= 0x2000 & _PPUAddress <= 0x2FFF)
                        {
                            ppuReadBuffer = chrRomHandler.GetPPUByte(Clock, _PPUAddress );
                        }
                        else
                        {
                            ppuReadBuffer = chrRomHandler.GetPPUByte(Clock, _PPUAddress & 0x3FFF);
                        }
                    }
                    if ((PPUControlByte0 & 0x4) == 0x4)
                    {
                        _PPUAddress = _PPUAddress + 32;
                    }
                    else
                    {
                        _PPUAddress = _PPUAddress + 1;
                    }

                    _PPUAddress = (_PPUAddress & 0x3FFF);
                    return tmp;

            }
            //throw new NotImplementedException(string.Format("PPU.GetByte() recieved invalid address {0,4:x}", address));
            return 0;
        }


        protected byte[][] palCache = new byte[256][];

        public byte[][] PalCache
        {
            get { return palCache; }
            set { palCache = value; }
        }
        protected int currentPalette = 0;

        public int CurrentPalette
        {
            get { return currentPalette; }
        }

        protected virtual void ClearNESPalette()
        {
            currentPalette = 0;
            palCache[currentPalette] = new byte[32];
            Buffer.BlockCopy(_palette, 0, palCache[currentPalette], 0, 32);
        }

        protected virtual void WriteToNESPalette(int address, byte data)
        {

            palCache[currentPalette] = new byte[32];
            Buffer.BlockCopy(_palette, 0, palCache[currentPalette], 0, 32);
            currentPalette++;

            int palAddress = (address) & 0x1F;
            _palette[palAddress] = data;
            // rgb32OutBuffer[255 * 256 + palAddress] = data;
            if ((_PPUAddress & 0xFFEF) == 0x3F00)
            {
                _palette[(palAddress ^ 0x10) & 0x1F] = data;
                // rgb32OutBuffer[255 * 256 + palAddress ^ 0x10] = data;
            }
            
        }


        #region IClockedMemoryMappedIOElement Members


        private MachineEvent nmiHandler = null;

        public MachineEvent NMIHandler
        {
            get
            {
                return nmiHandler;
            }
            set
            {
                nmiHandler = value;
            }
        }

        /// <summary>
        /// ppu doesnt throw irq's
        /// </summary>
        public bool IRQAsserted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int NextEventAt
        {
            get
            {
                //if (frameClock > 6823)
                //{

                     return (89345 - frameClock) / 3;
                //}
                //else
                //{
                //    return (6823 - frameClock) / 3;
                //}
            }
        }

        public void HandleEvent(int Clock)
        {
            DrawTo(Clock);
        }

        public void ResetClock(int Clock)
        {
            lastcpuClock = Clock;
        }

        #endregion

        MachineEvent frameFinished;

        public MachineEvent FrameFinishHandler
        {
            get { return frameFinished; }
            set { frameFinished = value;  }
        }

    }
}
