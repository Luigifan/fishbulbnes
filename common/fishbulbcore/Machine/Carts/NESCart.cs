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
        private new byte[] prgRomBank6 = new byte[0x800];

        // prg roms

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
                        CopyBanks(0, 0, 0, 1);
                    }
                    
                    SetupBankStarts(0, 1, PrgRomCount * 2 - 2, PrgRomCount * 2 - 1);
                    break;

                case 7:

                    //SetupBanks(0, 1, 2, 3);
                    SetupBankStarts(0, 1, 2, 3);
                    Mirror(0, 0);
                    break;

                default:
                    throw new NotImplementedException("Mapper " + mapperId.ToString() + " not implemented.");
            }
        }

        private int[] prevBSSrc = new int[8];

        // copy from dest to dest + count

        // note, this function originally worked with 8k banks
        private void CopyBanks(int clock, int dest, int src, int numberOf8kBanks)
        {
            whizzler.DrawTo(clock);
            
            if (dest >= ChrRomCount) dest = ChrRomCount - 1;
            
            int oneKsrc = src * 8;
            int oneKdest = dest * 8;
            //TODO: get whizzler reading ram from INesCart.GetPPUByte then be calling this
            //  setup ppuBankStarts in 0x400 block chunks 
            for (int i = 0; i < (numberOf8kBanks * 8); ++i)
            {
                ppuBankStarts[oneKdest + i] = (oneKsrc + i) * 0x400;
                if (oneKdest + i == 8)
                {
                    chrRamStart = (oneKsrc + i) * 0x400;
                }
            }
            Mirror(-1, mirroring);

            bankSwitchesChanged = true;
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

                SetupBankStarts(newbank8, newbank8 + 1, newbank8 + 2, newbank8 + 3);
                // whizzler.DrawTo(clock);
                if ((val & 16) == 16)
                {
                    OneScreenOffset = 0x400;
                }
                else
                {
                    OneScreenOffset = 0;
                }
                Mirror(clock, 0);
            }

            if (mapperId == 3 && address >= 0x8000)
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
