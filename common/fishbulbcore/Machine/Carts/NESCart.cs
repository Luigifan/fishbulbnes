using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NES.CPU.Fastendo;
using NES.CPU.Machine.Carts;

namespace NES.CPU
{
    public class NESCart : BaseCart
    {

        // sram
        private byte[] prgRomBank6 = new byte[0x800];

        // prg roms

        private int  mirroring, fourScreen, trainer;

        public override void InitializeCart()
        {

            for (int i = 0; i < 8; ++i)
                prevBSSrc[i] = -1;
            //SRAMEnabled = SRAMCanSave;


            switch (mapperId)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    if (ChrRomCount > 0)
                    {
                        CopyBanks(0, 0, 0, 2);
                    }
                    // SetupBanks(0, 1, prgRomCount * 2 - 2, prgRomCount * 2 - 1);
                    SetupBankStarts(0, 1, PrgRomCount * 2 - 2, PrgRomCount * 2 - 1);
                    break;

                case 7:

                    //SetupBanks(0, 1, 2, 3);
                    SetupBankStarts(0, 1, 2, 3);
                    whizzler.Mirroring = 0;
                    break;

                default:
                    throw new NotImplementedException("Mapper " + mapperId.ToString() + " not implemented.");
            }
        }

        private int[] prevBSSrc = new int[8];

        // copy from dest to dest + count
        private void CopyBanks(int clock, int dest, int src, int numberOf8kBanks)
        {
            whizzler.DrawTo(clock);
            if (dest >= ChrRomCount) dest = ChrRomCount - 1;
            ppuBankStarts[dest] = src * 0x2000;
            ppuBankStarts[dest + 1] = src * 0x2000 + 0x1000;

// Array.Copy(chrRom, src * 0x2000, whizzler.cartCopyVidRAM, dest * 0x2000, numberOf8kBanks * 0x2000);
        }

        #region IMemoryMappable Members

        public override void SetByte(int clock, int address, int val)
        {
            if (address >= 0x6000 && address <= 0x7FFF)
            {
                if (SRAMEnabled)
                {
                    prgRomBank6[address & 0x1FFF] = (byte)val;
                }

                return;
            }

            if (mapperId == 7)
            {
                // val selects which bank to swap, 32k at a time
                int newbank8 = 0;
                newbank8 = 4 * (val & 0xF);

                // SetupBanks(newbank8, newbank8 + 1, newbank8 + 2, newbank8 + 3);
                SetupBankStarts(newbank8, newbank8 + 1, newbank8 + 2, newbank8 + 3);
                whizzler.DrawTo(clock);
                if ((val & 16) == 16)
                {
                    whizzler.OneScreenMirrorOffset = 0x400;
                }
                else
                {
                    whizzler.OneScreenMirrorOffset = 0;
                }
                whizzler.Mirroring = 0;
            }

            if (mapperId == 3)
            {
                CopyBanks(clock, 0, val, 1);
            }

            if (mapperId == 2 && address >= 0x8000)
            {
                int newbank8 = 0;

                newbank8 = ((val) * 2);
                // keep two high banks, swap low banks

                // SetupBanks(newbank8, newbank8 + 1, currentC, currentE);
                SetupBankStarts(newbank8, newbank8 + 1, currentC, currentE);
            }





        }

        #endregion

    }
}
