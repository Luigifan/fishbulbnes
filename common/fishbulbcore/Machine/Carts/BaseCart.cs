using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.PPUClasses;
using System.Security.Cryptography;
using NES.CPU.Fastendo;
using NES.CPU.PixelWhizzlerClasses;

namespace NES.CPU.Machine.Carts
{
    public delegate string ROMHashFunctionDelegate (byte[] prg, byte[] chr);

    public delegate void ReadMySRAM(string romID, ref byte[] rams);

    public delegate void WriteMySRAM(string romID, ref byte[] rams);

    public abstract class BaseCart : INESCart
    {

        private Dictionary<int, byte[]> pixelEffects = new Dictionary<int, byte[]>();

        public BaseCart()
        {
            for (int i = 0; i < bankStartCache.Length; ++i)
            {
                bankStartCache[i] = new int[16];
            }

            byte[] effect = new byte[8] { 1, 1, 1, 1, 1, 1, 1, 1};
            pixelEffects.Add(0xD50, effect);
            pixelEffects.Add(0x0, effect);

            for (int i = 21264; i < 21696; i++)
            {
                pixelEffects.Add(i - 16400, effect);
            }

            for (int i = 0; i < 16; ++i)
            {
                ppuBankStarts[i] = i * 0x400;
            }
        }

        #region INESCart Members

        internal byte[] iNesHeader = new byte[16];
        internal byte[] romControlBytes = new byte[2];
        internal byte[] nesCart;
        private byte[] chrRom;

        public byte[] ChrRom
        {
            get { return chrRom; }
            set { chrRom = value; }
        }

        /// taken from basicNES
        internal int current8 = -1;
        internal int currentA = -1;
        internal int currentC = -1;
        internal int currentE = -1;

        internal bool SRAMCanWrite;
        internal bool SRAMEnabled;
        internal bool SRAMCanSave;

        internal int prgRomCount, chrRomCount;

        public int ChrRomCount
        {
            get { return chrRomCount; }
        }

        public int PrgRomCount
        {
            get { return prgRomCount; }
        }

        internal int mapperId;

        internal int bank8start, bankAstart, bankCstart, bankEstart;
        internal byte[] prgRomBank6 = new byte[0x2000];

        private ROMHashFunctionDelegate _ROMHashfunction;

        public ROMHashFunctionDelegate ROMHashFunction
        {
            get { return _ROMHashfunction; }
            set { _ROMHashfunction = value; }
        }

        int chrRomOffset = 0;

        protected int chrRamStart = 0;

        public void LoadiNESCart(byte[] header, int prgRoms, int chrRoms, byte[] prgRomData, byte[] chrRomData, int chrRomOffset)
        {
            romControlBytes[0] = header[6];
            romControlBytes[1] = header[7];

            mapperId = (romControlBytes[0] & 0xF0) >> 4;
            mapperId += romControlBytes[1] & 0xF0;
            this.chrRomOffset = chrRomOffset;
            /*
             .NES file format
            ---------------------------------------------------------------------------
            0-3      String "NES^Z" used to recognize .NES files.
            4        Number of 16kB ROM banks.
            5        Number of 8kB VROM banks.
            6        bit 0     1 for vertical mirroring, 0 for horizontal mirroring
                     bit 1     1 for battery-backed RAM at $6000-$7FFF
                     bit 2     1 for a 512-byte trainer at $7000-$71FF
                     bit 3     1 for a four-screen VRAM layout 
                     bit 4-7   Four lower bits of ROM Mapper Type.
            7        bit 0-3   Reserved, must be zeroes!
                     bit 4-7   Four higher bits of ROM Mapper Type.
            8-15     Reserved, must be zeroes!
            16-...   ROM banks, in ascending order. If a trainer i6s present, its
                     512 bytes precede the ROM bank contents.
            ...-EOF  VROM banks, in ascending order.
            ---------------------------------------------------------------------------
            */

            Array.Copy(header, iNesHeader, header.Length);
            prgRomCount = prgRoms;
            chrRomCount = chrRoms;

            nesCart = new byte[prgRomData.Length];
            Array.Copy(prgRomData, nesCart, prgRomData.Length);

            if (chrRomCount == 0)
            {
                // chrRom is going to be RAM
                chrRomData = new byte[0x8000];
            }

            
            chrRom = new byte[chrRomData.Length + 0x1000];

            chrRamStart = chrRomData.Length;

            Buffer.BlockCopy(chrRomData, 0, chrRom, 0, chrRomData.Length);

            prgRomCount = iNesHeader[4];
            chrRomCount = iNesHeader[5];


            romControlBytes[0] = iNesHeader[6];
            romControlBytes[1] = iNesHeader[7];

            SRAMCanSave = (romControlBytes[0] & 0x02) == 0x02;
            SRAMEnabled = true;


            // rom0.0=0 is horizontal mirroring, rom0.0=1 is vertical mirroring

            // by default we have to call Mirror() at least once to set up the bank offsets
            Mirror(0, 0);
            if ((romControlBytes[0] & 0x01) == 1)
            {
                Mirror(0, 1);
            }
            else
            {
                Mirror(0, 2);
            }

            if ((romControlBytes[0] & 0x08) == 0x08)
            {
                Mirror(0, 3);
            }


            checkSum = ROMHashFunction(nesCart, chrRom);

            InitializeCart();

        }

        public abstract void InitializeCart();

        protected IPPU whizzler;

        public IPPU Whizzler
        {
            get { return whizzler; }
            set { whizzler = value; }
        }

        public bool irqRaised;

        public bool IrqRaised
        {
            get { return irqRaised; }
            set { irqRaised = value; }
        }


        #endregion

        #region IMemoryMappedIOElement Members

        public virtual void UpdateScanlineCounter() {}

        public virtual int GetByte(int clock, int address)
        {
            int bank = 0;

            switch (address & 0xE000)
            {
                // i dont think these cart types need to implement persistant saveram
                case 0x6000:
                    return prgRomBank6[address & 0x1FFF];

                case 0x8000:
                    bank = bank8start;
                    break;
                case 0xA000:
                    bank = bankAstart;
                    break;
                case 0xC000:
                    bank = bankCstart;
                    break;
                case 0xE000:
                    bank = bankEstart;
                    break;
            }
            // if cart is half sized, adjust
            if (bank + (address & 0x1FFF) > nesCart.Length)
            {
                throw new Exception("THis is broken!");
            }
            return nesCart[bank + (address & 0x1FFF)];


        }

        public abstract void SetByte(int clock, int address, int data);

        #endregion


        internal void SetupBankStarts(int reg8, int regA, int regC, int regE)
        {
            reg8 = MaskBankAddress(reg8);
            regA = MaskBankAddress(regA);
            regC = MaskBankAddress(regC);
            regE = MaskBankAddress(regE);

            current8 = reg8;
            currentA = regA;
            currentC = regC;
            currentE = regE;
            bank8start = reg8 * 0x2000;
            bankAstart = regA * 0x2000;
            bankCstart = regC * 0x2000;
            bankEstart = regE * 0x2000;

        }

        internal virtual int MaskBankAddress(int bank)
        {
            if (bank >= prgRomCount * 2)
            {
                int i = 0xFF;
                while ((bank & i) >= prgRomCount * 2)
                {
                    i = i >> 1;
                }
                return (bank & i);
            }
            else
            {
                return bank;
            }
        }


        #region INESCart Members

        string checkSum;

        public string CheckSum
        {
            get { return checkSum; }
        }

        public virtual void WriteState(Queue<int> state)
        {
            /// taken from basicNES
            state.Enqueue( SRAMCanWrite ? 1 : 0);
            state.Enqueue (SRAMEnabled ? 1: 0);
            state.Enqueue (SRAMCanSave ? 1: 0);

            state.Enqueue(prgRomCount);
            state.Enqueue(chrRomCount);

            state.Enqueue(mapperId);

            state.Enqueue(bank8start);
            state.Enqueue(bankAstart);
            state.Enqueue(bankCstart);
            state.Enqueue(bankEstart);

            for (int i = 0; i < 0x2000; i += 4)
            {

                state.Enqueue((prgRomBank6[i] << 24) |
                                    (prgRomBank6[i + 1] << 16) |
                                    (prgRomBank6[i + 2] << 8) |
                                    (prgRomBank6[i + 3])
                        );
            }
        }

        public virtual void ReadState(Queue<int> state)
        {
            /// taken from basicNES
            SRAMCanWrite = state.Dequeue() == 1;
            SRAMEnabled = state.Dequeue() == 1;
            SRAMCanSave = state.Dequeue() == 1;

            prgRomCount = state.Dequeue();
            chrRomCount = state.Dequeue();

            mapperId = state.Dequeue();

            bank8start = state.Dequeue();
            bankAstart = state.Dequeue();
            bankCstart = state.Dequeue();
            bankEstart = state.Dequeue();

            int packedByte = 0;
            for (int i = 0; i < 0x2000; i += 4)
            {
                packedByte = state.Dequeue();
                prgRomBank6[i] = (byte)(packedByte >> 24);
                prgRomBank6[i + 1] = (byte)(packedByte >> 16);
                prgRomBank6[i + 2] = (byte)(packedByte >> 8);
                prgRomBank6[i + 3] = (byte)(packedByte);

            }
        }

        #endregion

        #region INESCart Members

        public NES.CPU.Fastendo.CPU2A03 CPU
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

        public byte[] SRAM
        {
            get
            {
                return prgRomBank6;
            }
            set
            {
                if (value != null && value.Length == prgRomBank6.Length)
                {
                    prgRomBank6 = value;
                }
            }
        }

        #endregion

        protected int mirroring = -1;

        public string CartName { get; protected set; }
        public int NumberOfPrgRoms { get { return prgRomCount; } }
        public int NumberOfChrRoms { get { return chrRomCount; } }
        public int MapperID { get { return mapperId; } }
        public NameTableMirroring Mirroring { get { return (NameTableMirroring)mirroring; } }

        #region IClockedMemoryMappedIOElement Members

        internal MachineEvent updateIRQ;

        public MachineEvent NMIHandler
        {
            get
            {
                return updateIRQ;
            }
            set
            {
                updateIRQ = value;
            }
        }

        public virtual bool IRQAsserted
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public virtual int NextEventAt
        {
            get { return -1; }
        }

        public virtual void HandleEvent(int Clock)
        {
        }

        public virtual void ResetClock(int Clock)
        {
        }

        #endregion

        protected int[] ppuBankStarts = new int[16];

        public int[] PpuBankStarts
        {
            get { return ppuBankStarts; }
            set { ppuBankStarts = value; }
        }

        int[][] bankStartCache = new int[256 * 256][];

        public int[][] BankStartCache
        {
            get { return bankStartCache; }
        }

        uint currentBank = 0;

        public uint CurrentBank
        {
            get { return currentBank; }
        }
        public void ResetBankStartCache()
        {
            // if (currentBank > 0)
            currentBank = 0;
            Array.Copy(ppuBankStarts, 0, bankStartCache[0], 0, 16);

            //Mirror(-1, this.mirroring);
            //chrRamStart = ppuBankStarts[8];
            //Buffer.BlockCopy(ppuBankStarts, 0, bankStartCache[0], 0, 16 * 4);
            //bankSwitchesChanged = false;
        }
        
        //gets called by the ppu 
        public int UpdateBankStartCache()
        {
            if (bankSwitchesChanged)
            {

                Array.Copy(ppuBankStarts, 0, bankStartCache[currentBank], 0, 16);
                currentBank++;
                bankSwitchesChanged = false;
            }
            return (int)currentBank;
        }

        protected bool bankSwitchesChanged = false;

        public bool BankSwitchesChanged
        {
            get { return bankSwitchesChanged; }
            set { bankSwitchesChanged = value; }
        }

        public byte GetPPUByte(int clock, int address)
        {
            int bank = address / 0x400;
            int newAddress = ppuBankStarts[bank] + (address & 0x3FF);

            //while (newAddress > chrRamStart)
            //{
            //    newAddress -= chrRamStart;
            //}
            return chrRom[newAddress];
        }
        
        // returns the absolute location of this address from the base of chrRom
        public int ActualChrRomOffset(int address)
        {
            int bank = address / 0x400;
            int newAddress = ppuBankStarts[bank] + (address & 0x3FF);
            return newAddress;
        }

        public void SetPPUByte(int clock, int address, byte data)
        {
            int bank = address / 0x400;
            int newAddress = ppuBankStarts[bank] + (address & 0x3FF);
            chrRom[newAddress] = data;
        }

        byte[] nullEffect = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

        public byte[] FetchPixelEffect(int vramAddress)
        {
            int bank = vramAddress / 0x400;
            int newAddress = ppuBankStarts[bank] + (vramAddress & 0x3FF);

            if (pixelEffects.ContainsKey(newAddress))
                return pixelEffects[newAddress];
            else
                return nullEffect;

        }

        int oneScreenOffset;
        internal int OneScreenOffset
        {
            get { return oneScreenOffset; }
            set { oneScreenOffset = value; }
        }

        internal void Mirror(int clockNum, int mirroring)
        {
            //    //            A11 A10 Effect
            //    //----------------------------------------------------------
            //    // 0   0  All four screen buffers are mapped to the same
            //    //        area of memory which repeats at $2000, $2400,
            //    //        $2800, and $2C00.
            //    // 0   x  "Upper" and "lower" screen buffers are mapped to
            //    //        separate areas of memory at $2000, $2400 and
            //    //        $2800, $2C00. ( horizontal mirroring)
            //    // x   0  "Left" and "right" screen buffers are mapped to
            //    //        separate areas of memory at $2000, $2800 and
            //    //        $2400,$2C00.  (vertical mirroring)
            //    // x   x  All four screen buffers are mapped to separate
            //    //        areas of memory. In this case, the cartridge
            //    //        must contain 2kB of additional VRAM (i got vram up the wazoo)
            //    // 0xC00 = 110000000000
            //    // 0x800 = 100000000000
            //    // 0x400 = 010000000000
            //    // 0x000 = 000000000000

            // if (mirroring == this.mirroring) return;

            this.mirroring = mirroring;
            
            if (clockNum > -1)
                whizzler.DrawTo(clockNum);

            //Console.WriteLine("Mirroring set to {0}", mirroring);

            switch (mirroring)
            {
                // onescreen mirroring, all four banks point to the same spot (which is defined by the offset)
                case 0:
                    ppuBankStarts[8] = chrRamStart + 0x0 + oneScreenOffset;
                    ppuBankStarts[9] = chrRamStart + 0x0 + oneScreenOffset;
                    ppuBankStarts[10] = chrRamStart + 0x0 + oneScreenOffset;
                    ppuBankStarts[11] = chrRamStart + 0x0 + oneScreenOffset;
                    break;
                // vertical and horizontal mirror modes share the same 2k rams, just in different ways.  That's all the nes has
                case 1:
                    ppuBankStarts[8] = chrRamStart + 0x0;
                    ppuBankStarts[9] = chrRamStart + 0x400;
                    ppuBankStarts[10] = chrRamStart + 0x000;
                    ppuBankStarts[11] = chrRamStart + 0x400;
                    break;
                case 2:
                    ppuBankStarts[8] = chrRamStart + 0x000;
                    ppuBankStarts[9] = chrRamStart + 0x000;
                    ppuBankStarts[10] = chrRamStart + 0x400;
                    ppuBankStarts[11] = chrRamStart + 0x400;
                    break;
                // fourscreen mirroring uses the extra 2k (on the 'cart') from 0x2800-0x2FFF
                case 3:
                    ppuBankStarts[8] = chrRamStart + 0x000;
                    ppuBankStarts[9] = chrRamStart + 0x400;
                    ppuBankStarts[10] = chrRamStart + 0x800;
                    ppuBankStarts[11] = chrRamStart + 0xC00;
                    break;
            }
            bankSwitchesChanged = true;
        }

        #region INESCart Members


        public int ChrRamStart
        {
            get { return chrRamStart; }
        }

        public int[] PPUBankStarts
        {
            get
            {
                return ppuBankStarts;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
