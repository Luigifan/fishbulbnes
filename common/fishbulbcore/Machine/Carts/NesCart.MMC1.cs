using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using System.IO;
using NES.CPU.PPUClasses;

namespace NES.CPU.Machine.Carts
{
        public class NesCartMMC1 : BaseCart
        {

            int sequence = 0;

            int accumulator = 0;
            int bank_select = 0;

            int[] _registers = new int[4];


            // sram

            public override void InitializeCart()
            {

                if (ChrRomCount > 0)
                {
                    CopyBanks(0, 0, 4);
                }
                _registers[0] = 0x0C;
                _registers[1] = 0x00;
                _registers[2] = 0x00;
                _registers[3] = 0x00;
                
                SetupBankStarts(0, 1, PrgRomCount * 2 - 2, PrgRomCount * 2 - 1);

                sequence = 0; accumulator = 0;
            }

 
            public int MaskBankAddress(int bank)
            {
                if (bank >= PrgRomCount * 2)
                {
                    int i; i = 0xFF;
                    while ((bank & i) >= PrgRomCount * 2)
                    {

                        i = i / 2;
                    }

                    return (bank & i);
                }
                else
                {
                    return bank;
                }
            }


            // copy from dest to dest + count
            private void CopyBanks(int dest, int src, int numberOf4kBanks)
            {
                whizzler.DrawTo(lastClock);
                if (ChrRomCount > 0)
                {
                    int oneKdest = dest * 4;
                    int oneKsrc = src * 4;
                    //TODO: get whizzler reading ram from INesCart.GetPPUByte then be calling this
                    //  setup ppuBankStarts in 0x400 block chunks 
                    for (int i = 0; i < (numberOf4kBanks * 4); ++i)
                        ppuBankStarts[oneKdest + i] = (oneKsrc + i) * 0x400;

                    //Array.Copy(chrRom, src * 0x1000, whizzler.cartCopyVidRAM, dest * 0x1000, numberOf4kBanks * 0x1000);
                }
            }



            #region IMemoryMappable Members
            int lastwriteAddress = 0;
            int lastClock ;
            public override void SetByte(int clock, int address, int val)
            {
                // if write is to a different register, reset
                lastClock = clock;
                switch (address & 0xF000)
                {
                    case 0x6000:
                    case 0x7000:
                        prgRomBank6[address & 0x1FFF] = (byte)val;
                        break;

                    default:
                        lastwriteAddress = address;
                        if ((val & 0x80) == 0x80)
                        {
                            _registers[0] = _registers[0] | 0xC;
                            accumulator = 0; // _registers[(address / 0x2000) & 3];
                            sequence = 0;
                        }
                        else
                        {
                            if ((val & 1) == 1)
                            {
                                accumulator = accumulator | (1 << sequence);
                            }
                            sequence = sequence + 1;
                        }

                        if (sequence == 5)
                        {
                            int regnum = (address & 0x7FFF) >> 13;
                            _registers[(address & 0x7FFF) >> 13] = accumulator;
                            sequence = 0;
                            accumulator = 0;

                            switch (regnum)
                            {
                                case 0:
                                    SetMMC1Mirroring(clock);
                                    break;
                                case 1:
                                case 2:
                                    SetMMC1ChrBanking(clock);
                                    break;
                                case 3:
                                    SetMMC1PrgBanking();
                                    break;
                            }

                        }
                        break;
                }

            }

            private void SetMMC1ChrBanking(int clock)
            {
                //	bit 4 - sets 8KB or 4KB CHRROM switching mode
                // 0 = 8KB CHRROM banks, 1 = 4KB CHRROM banks
                whizzler.DrawTo(clock);
                if ((_registers[0] & 0x10) == 0x10)
                {
                    CopyBanks(0, _registers[1], 1);
                    CopyBanks(1, _registers[2], 1);
                }
                else
                {
                    //CopyBanks(0, _registers[1], 2);
                    CopyBanks(0, _registers[1], 1);
                    CopyBanks(1, _registers[1] + 1, 1);
                }

            }

            private void SetMMC1PrgBanking()
            {
                int reg;
                if (PrgRomCount == 0x20) // 512k cart
                {
                    bank_select = (_registers[1] & 0x10) << 1;

                }
                else
                {
                    bank_select = 0;
                }


                if ((_registers[0] & 8) == 0) // swap entire 32k bank
                {
                    reg = 4 * ((_registers[3] >> 1) & 0xF) + bank_select;
                    SetupBankStarts(reg, reg + 1, reg + 2, reg + 3);
                }
                else
                {
                    reg = 2 * (_registers[3] ) + bank_select;
                    //bit 2 - toggles between low PRGROM area switching and high
                    //PRGROM area switching
                    //0 = high PRGROM switching, 1 = low PRGROM switching
                    if ((_registers[0] & 4) == 4) // if bit set, swap low bank, else high bank
                    {
                        // select 16k bank in register 3 (setupbankstarts switches 8k banks)
                        SetupBankStarts(reg, reg + 1, PrgRomCount * 2 - 2, PrgRomCount * 2 - 1);
                        //SetupBanks(reg8, reg8 + 1, 0xFE, 0xFF);
                    }
                    else
                    {
                        SetupBankStarts(0, 1, reg, reg + 1);
                    }
                }
            }

            private void SetMMC1Mirroring(int clock)
            {
                //bit 1 - toggles between H/V and "one-screen" mirroring
                //0 = one-screen mirroring, 1 = H/V mirroring
                whizzler.DrawTo(clock);
                switch (_registers[0] & 3)
                {
                    case 0:
                        whizzler.Mirroring = 0;
                        whizzler.OneScreenMirrorOffset = 0x0;
                        break;
                    case 1:
                        whizzler.Mirroring = 0;
                        whizzler.OneScreenMirrorOffset = 0x400;
                        break;
                    case 2:
                        whizzler.Mirroring = 1; // vertical
                        break;
                    case 3:
                        whizzler.Mirroring = 2; // horizontal
                        break;
                }
            }

            #endregion


        }

}
