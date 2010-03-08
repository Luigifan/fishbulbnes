using System;
using NES.CPU.Fastendo;
using System.Collections.Generic;
using NES.CPU.PixelWhizzlerClasses;
namespace NES.CPU.Machine.Carts
{
    public interface INESCart : IClockedMemoryMappedIOElement
    {
        void LoadiNESCart(byte[] header, int prgRoms, int chrRoms, byte[] prgRomData, byte[] chrRomData, int chrRomOffset);

        IPPU Whizzler { get; set; }
        CPU2A03 CPU { get; set; }

        void InitializeCart();

        void UpdateScanlineCounter();

        void WriteState(Queue<int> state);
        void ReadState(Queue<int> state);

        byte[] ChrRom { get; set; }
        int ChrRamStart { get; }
        int[] PPUBankStarts { get; set; }

        ROMHashFunctionDelegate ROMHashFunction { get; set; }
        string CheckSum { get;  }

        byte[] SRAM { get; set; }

        NameTableMirroring Mirroring { get;  }
        string CartName { get;  }
        int NumberOfPrgRoms { get;  }
        int NumberOfChrRoms { get;  }
        int MapperID { get;  }

        byte GetPPUByte(int clock, int address);
        void SetPPUByte(int clock, int address, byte data);

        byte[] FetchPixelEffect(int vramAddress);
        int ActualChrRomOffset(int address);

        /// <summary>
        /// Used for bankswitching
        /// </summary>
        bool BankSwitchesChanged { get; set; }
        int UpdateBankStartCache();
        void ResetBankStartCache();
        int[] BankStartCache { get; }
        int CurrentBank { get; }

    }
}
