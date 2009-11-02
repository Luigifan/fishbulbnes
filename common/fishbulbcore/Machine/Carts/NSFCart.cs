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
            for (int i = 0; i < 8; ++i)
            {
                bankInitVals[i] = header[0x70 + i];
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

        #region INESCart Members


        public byte GetPPUByte(int clock, int address)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INESCart Members


        public byte[] FetchPixelEffect(int vramAddress)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INESCart Members


        public void SetPPUByte(int clock, int address, byte data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
