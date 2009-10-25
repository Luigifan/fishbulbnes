using System;
using NES.CPU.Fastendo;
using System.Collections.Generic;
namespace NES.CPU.Machine.Carts
{
    public interface INESCart : IClockedMemoryMappedIOElement
    {
        void LoadiNESCart(byte[] header, int prgRoms, int chrRoms, byte[] prgRomData, byte[] chrRomData);

        NES.CPU.PPUClasses.PixelWhizzler Whizzler { get; set; }
        CPU2A03 CPU { get; set; }

        bool IrqRaised { get; set; }

        void InitializeCart();

        void UpdateScanlineCounter();

        void WriteState(Queue<int> state);
        void ReadState(Queue<int> state);

        ROMHashFunctionDelegate ROMHashFunction { get; set; }
        string CheckSum { get;  }

        byte[] SRAM { get; set; }

        NameTableMirroring Mirroring { get;  }
        string CartName { get;  }
        int NumberOfPrgRoms { get;  }
        int NumberOfChrRoms { get;  }
        int MapperID { get;  }

        byte GetPPUByte(int clock, int address);

    }
}
