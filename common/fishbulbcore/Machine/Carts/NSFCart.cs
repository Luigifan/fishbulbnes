using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;

namespace NES.CPU.Machine.Carts
{
    public class NSFCart : INESCart
    {
        byte[] roms;
        byte[] sram = new byte[0x800];
        int[] bankStarts = new int[8];
        byte[][] banks;
        int[] bankInitVals = new int[8];
        int numSongs;
        int startSong;
        ushort loadAddress, initAddress, playAddress;
        string songname, artist, copyright;

        byte[] BIOS = new byte[256]
        {
        	0xFF,0xFF,0xFF,0x20,0x33,0x3F,0x78,0xA2,0xFF,0x8E,0x17,0x40,0xE8,0x8E,0x15,0x40,
        	0x8E,0x00,0x20,0x8E,0x01,0x20,0x8E,0x12,0x3E,0x58,0x4C,0x1A,0x3F,0x48,0x8A,0x48,
        	0x98,0x48,0xAE,0x12,0x3E,0xF0,0x56,0xCA,0xF0,0xD9,0x20,0xF9,0x3F,0x68,0xA8,0x68,
        	0xAA,0x68,0x40,0xAD,0x13,0x3E,0x4A,0x90,0x09,0x8E,0x02,0x90,0x8E,0x02,0xA0,0x8E,
        	0x02,0xB0,0x4A,0x90,0x0D,0xA0,0x20,0x8C,0x10,0x90,0x8E,0x30,0x90,0xC8,0xC0,0x26,
        	0xD0,0xF5,0x4A,0x90,0x0B,0xA0,0x80,0x8C,0x83,0x40,0x8C,0x87,0x40,0x8C,0x89,0x40,
        	0x4A,0x90,0x03,0x8E,0x15,0x50,0x4A,0x90,0x08,0xCA,0x8E,0x00,0xF8,0xE8,0x8E,0x00,
        	0x48,0x4A,0x90,0x08,0xA0,0x07,0x8C,0x00,0xC0,0x8C,0x00,0xE0,0x60,0x20,0x33,0x3F,
        	0x8A,0xCA,0x9A,0x8E,0xF7,0x5F,0xCA,0x8E,0xF6,0x5F,0xA2,0x7F,0x85,0x00,0x86,0x01,
        	0xA8,0xA2,0x27,0x91,0x00,0xC8,0xD0,0xFB,0xCA,0x30,0x0A,0xC6,0x01,0xE0,0x07,0xD0,
        	0xF2,0x86,0x01,0xF0,0xEE,0xA2,0x14,0xCA,0x9D,0x00,0x40,0xD0,0xFA,0xA2,0x07,0xBD,
        	0x08,0x3E,0x9D,0xF8,0x5F,0xCA,0x10,0xF7,0xA0,0x0F,0x8C,0x15,0x40,0xAD,0x13,0x3E,
        	0x29,0x04,0xF0,0x10,0xAD,0x0E,0x3E,0xF0,0x03,0x8D,0xF6,0x5F,0xAD,0x0F,0x3E,0xF0,
        	0x03,0x8D,0xF7,0x5F,0xAE,0x11,0x3E,0xBD,0x04,0x3E,0x8D,0x10,0x3E,0xBD,0x06,0x3E,
        	0x8D,0x11,0x3E,0x8C,0x12,0x3E,0xAD,0x12,0x3E,0x58,0xAD,0x10,0x3E,0x20,0xF6,0x3F,
        	0x8D,0x13,0x3E,0x4C,0x1A,0x3F,0x6C,0x00,0x3E,0x6C,0x02,0x3E,0x06,0x3F,0x1D,0x3F
        };

        #region INESCart Members

        public void LoadiNESCart(byte[] header, int prgRoms, int chrRoms, byte[] prgRomData, byte[] chrRomData, int chrRomOffset)
        {

            banks = new byte[8][];
            for (int i = 0; i < 8; ++i)
            {
                banks[i] = new byte[0x1000];
            }
            //0000    5   STRING  "NESM",01Ah  ; denotes an NES sound format file
            //0005    1   BYTE    Version number (currently 01h)
            //0006    1   BYTE    Total songs   (1=1 song, 2=2 songs, etc)
            numSongs = header[0x6];
            //0007    1   BYTE    Starting song (1= 1st song, 2=2nd song, etc)
            startSong = header[0x7];
            //0008    2   WORD    (lo/hi) load address of data (8000-FFFF)
            loadAddress = (ushort)(header[0x8] | (header[0x9] << 8));
            //000a    2   WORD    (lo/hi) init address of data (8000-FFFF)
            for (int i = 0; i < 8; ++i)
            {
                bankStarts[i] = (loadAddress + (i * 0x1000)) & 0xFFFF;
            }
            initAddress = (ushort)(header[0xa] | (header[0xb] << 8));
            //000c    2   WORD    (lo/hi) play address of data (8000-FFFF)
            playAddress = (ushort)(header[0xc] | (header[0xd] << 8));
            //000e    32  STRING  The name of the song, null terminated
            songname = "unk";
            //002e    32  STRING  The artist, if known, null terminated
            //004e    32  STRING  The Copyright holder, null terminated
            //006e    2   WORD    (lo/hi) speed, in 1/1000000th sec ticks, NTSC (see text)
            //0070    8   BYTE    Bankswitch Init Values (see text, and FDS section)
            bool bankSwitchUsed = false;
            for (int i = 0; i < 8; ++i)
            {
                bankInitVals[i] = header[0x70 + i];
                if (bankInitVals[i] != 0)
                {
                    bankSwitchUsed = true;
                }
            }


            //0078    2   WORD    (lo/hi) speed, in 1/1000000th sec ticks, PAL (see text)
            //007a    1   BYTE    PAL/NTSC bits:
            //                 bit 0: if clear, this is an NTSC tune
            //                 bit 0: if set, this is a PAL tune
            //                 bit 1: if set, this is a dual PAL/NTSC tune
            //                 bits 2-7: not used. they *must* be 0
            //007b    1   BYTE    Extra Sound Chip Support
            //                 bit 0: if set, this song uses VRCVI
            //                 bit 1: if set, this song uses VRCVII
            //                 bit 2: if set, this song uses FDS Sound
            //                 bit 3: if set, this song uses MMC5 audio
            //                 bit 4: if set, this song uses Namco 106
            //                 bit 5: if set, this song uses Sunsoft FME-07
            //                 bits 6,7: future expansion: they *must* be 0
            //007c    4   ----    4 extra bytes for expansion (must be 00h)
            //0080    nnn ----    The music program/data follows
            roms = prgRomData;
            
            // throw new NotImplementedException();
        }

        public NES.CPU.PPUClasses.PixelWhizzler Whizzler
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        CPU2A03 cpu;

        public CPU2A03 CPU
        {
            get
            {
                return cpu;
            }
            set
            {
                cpu = value;
            }
        }

        public bool IrqRaised
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public void InitializeCart()
        {
        }

        public void UpdateScanlineCounter()
        {
        }

        public void WriteState(Queue<int> state)
        {
        }

        public void ReadState(Queue<int> state)
        {
        }

        public ROMHashFunctionDelegate ROMHashFunction
        {
            get;
            set;
        }

        public string CheckSum
        {
            get { return null; }
        }

        public byte[] SRAM
        {
            get
            {
                return SRAM;
            }
            set
            {
                sram = value;
            }
        }

        public NameTableMirroring Mirroring
        {
            get { return NameTableMirroring.OneScreen; }
        }

        public string CartName
        {
            get { return "tunes"; }
        }

        public int NumberOfPrgRoms
        {
            get { return 0; }
        }

        public int NumberOfChrRoms
        {
            get { return 0; }
        }

        public int MapperID
        {
            get { return -1; }
        }

        #endregion

        #region IClockedMemoryMappedIOElement Members

        public int GetByte(int Clock, int address)
        {
            return roms[bankStarts[(((address & 0x8000) / 0x1000) - 8)] + (address - 0x8000)];
        }

        public void SetByte(int Clock, int address, int data)
        {
            if (address >= 0x5ff8 && address <= 0x5fff)
            {
                bankStarts[address - 0x5ff8] = data;
                UpdateBankSwitch();
            }
        }

        void UpdateBankSwitch()
        {
        }

        public NES.CPU.Fastendo.MachineEvent NMIHandler
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
            get { return -1; }
        }

        public void HandleEvent(int Clock)
        {
            throw new NotImplementedException();
        }

        public void ResetClock(int Clock)
        {
        }

        #endregion


        PixelWhizzlerClasses.IPPU INESCart.Whizzler
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

        public int ChrRamStart
        {
            get { throw new NotImplementedException(); }
        }

        public int[] PPUBankStarts
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

        public bool BankSwitchesChanged
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

        public int UpdateBankStartCache()
        {
            throw new NotImplementedException();
        }

        public void ResetBankStartCache()
        {
            throw new NotImplementedException();
        }

        public int[] BankStartCache
        {
            get { throw new NotImplementedException(); }
        }

        public int CurrentBank
        {
            get { throw new NotImplementedException(); }
        }


        public byte[] ChrRom
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

        public byte GetPPUByte(int clock, int address)
        {
            throw new NotImplementedException();
        }

        public void SetPPUByte(int clock, int address, byte data)
        {
            throw new NotImplementedException();
        }

        public byte[] FetchPixelEffect(int vramAddress)
        {
            throw new NotImplementedException();
        }

        public int ActualChrRomOffset(int address)
        {
            throw new NotImplementedException();
        }


        public bool UsesSRAM
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }
    }
}
