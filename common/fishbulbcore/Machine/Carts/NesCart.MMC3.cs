using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using System.IO;
using NES.CPU.PPUClasses;

namespace NES.CPU.Machine.Carts
{
        public class NesCartMMC3 : BaseCart
        {

            int[] _registers = new int[4];

            int chr2kBank0 = 0;
            int chr2kBank1 = 1;

            int chr1kBank0 = 0;
            int chr1kBank1 = 0;
            int chr1kBank2 = 0;
            int chr1kBank3 = 0;

            int prgSwap = 0;
            int prgSwitch1 = 0, prgSwitch2 = 0;


            public override void InitializeCart()
            {

                prgSwap = 1;

                //SetupBanks(0, 1, 0xFE, 0xFF);
                prgSwitch1 = 0;
                prgSwitch2 = 1;
                SwapPrgRomBanks();
                _mmc3IrqVal = 0; 
                _mmc3IrcOn = false;
                _mmc3TmpVal = 0;

                chr2kBank0 = 0;
                chr2kBank1 = 0;

                chr1kBank0 = 0;
                chr1kBank1 = 0;
                chr1kBank2 = 0;
                chr1kBank3 = 0;

                if (ChrRomCount > 0)
                {
                    CopyBanks(0, 0, 8);
                }
            }

            public int MaskBankAddress(int bank)
            {
                if (bank >= PrgRomCount * 2)
                {
                    int i = 0xFF;
                    while ((bank & i) >= PrgRomCount * 2)
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


            private int[] prevBSSrc = new int[8];

            // copy from dest to dest + count
            private void CopyBanks(int dest, int src, int numberOf1kBanks)
            {
                if (ChrRomCount > 0)
                {
                    Array.Copy(chrRom, src * 0x400, whizzler.cartCopyVidRAM, dest * 0x400, numberOf1kBanks * 0x400);
                }
            }

            #region IMemoryMappable Members


            public override bool IRQAsserted
            {
                get
                {
                    return irqRaised;
                }
                set
                {
                    irqRaised = value;
                }
            }


            int _mmc3Command = 0;
            int _mmc3ChrAddr = 0;

            int _mmc3IrqVal = 0;
            int _mmc3TmpVal = 0;
            int scanlineCounter = 0;
            bool _mmc3IrcOn = false;
            
            // 8 x 1k ppu banks, all switchable
            bool ppuBankSwap = false;
            int[] PPUBanks = new int[8];

            public override void SetByte(int clock, int address, int val)
            {
                if (address >= 0x6000 && address < 0x8000)
                {
                    if (SRAMEnabled && SRAMCanWrite)
                    {
                        prgRomBank6[address & 0x1FFF] = (byte)val;
                    }
                    return;
                }
                //Bank select ($8000-$9FFE, even)

                //7  bit  0
                //---- ----
                //CPxx xRRR
                //||    |||
                //||    +++- Specify which bank register to update on next write to Bank Data register
                            //_mmc3Command
                //||         0: Select 2 KB CHR bank at PPU $0000-$07FF (or $1000-$17FF);
                //||         1: Select 2 KB CHR bank at PPU $0800-$0FFF (or $1800-$1FFF);
                //||         2: Select 1 KB CHR bank at PPU $1000-$13FF (or $0000-$03FF);
                //||         3: Select 1 KB CHR bank at PPU $1400-$17FF (or $0400-$07FF);
                //||         4: Select 1 KB CHR bank at PPU $1800-$1BFF (or $0800-$0BFF);
                //||         5: Select 1 KB CHR bank at PPU $1C00-$1FFF (or $0C00-$0FFF);
                //||         6: Select 8 KB PRG bank at $8000-$9FFF (or $C000-$DFFF);
                //||         7: Select 8 KB PRG bank at $A000-$BFFF

                //|+-------- PRG ROM bank configuration (0: $8000-$9FFF swappable, $C000-$DFFF fixed to second-last bank;
                //|                                      1: $C000-$DFFF swappable, $8000-$9FFF fixed to second-last bank)
                //+--------- CHR ROM bank configuration (0: two 2 KB banks at $0000-$0FFF, four 1 KB banks at $1000-$1FFF;
                //                                       1: four 1 KB banks at $0000-$0FFF, two 2 KB banks at $1000-$1FFF)
                switch (address & 0xE001)
                {
                    case 0x8000:

                        _mmc3Command = val & 0x7;
                        if ((val & 0x80) == 0x80)
                        {
                            ppuBankSwap = true;
                            _mmc3ChrAddr = 0x1000;
                        }
                        else
                        {
                            ppuBankSwap = false;
                            _mmc3ChrAddr = 0x0;
                        }
                        if ((val & 0x40) == 0x40)
                        {
                            prgSwap = 1;
                        }
                        else
                        {
                            prgSwap = 0;
                        }
                        SwapPrgRomBanks();
                        break;
                    case 0x8001:
                        switch (_mmc3Command)
                        {
                            case 0:
                                chr2kBank0 = val;
                                SwapChrBanks();
                                // CopyBanks(0, val, 1);
                                // CopyBanks(1, val + 1, 1);
                                break;
                            case 1:
                                chr2kBank1 = val;
                                SwapChrBanks();
                                // CopyBanks(2, val, 1);
                                // CopyBanks(3, val + 1, 1);
                                break;
                            case 2:
                                chr1kBank0 = val;
                                SwapChrBanks();
                                //CopyBanks(4, val, 1);
                                break;
                            case 3:
                                chr1kBank1 = val;
                                SwapChrBanks();
                                //CopyBanks(5, val, 1);
                                break;
                            case 4:
                                chr1kBank2 = val;
                                SwapChrBanks();
                                //CopyBanks(6, val, 1);
                                break;
                            case 5:
                                chr1kBank3 = val;
                                SwapChrBanks();
                                //CopyBanks(7, val, 1);
                                break;
                            case 6:
                                prgSwitch1 = val;
                                SwapPrgRomBanks();
                                break;
                            case 7:
                                prgSwitch2 = val;
                                SwapPrgRomBanks();
                                break;

                        }
                        break;
                    case 0xA000:
                        if ((val & 1) == 1)
                        {
                            whizzler.Mirroring = 2;
                        }
                        else
                        {
                            whizzler.Mirroring = 1;
                        }
                        break;
                    case 0xA001:
                        //PRG RAM protect ($A001-$BFFF, odd)

                        //7  bit  0
                        //---- ----
                        //RWxx xxxx
                        //||
                        //|+-------- Write protection (0: allow writes; 1: deny writes)
                        //+--------- Chip enable (0: disable chip; 1: enable chip)
                        SRAMCanWrite = ((val & 0x40) == 0);
                        SRAMEnabled = ((val & 0x80) == 0x80);

                        break;
                    case 0xC000:
                        _mmc3IrqVal = val;
                        if (val == 0)
                        {
                            // special treatment for one-time irq handling
                            scanlineCounter = 0;
                        }
                        break;
                    case 0xC001:
                        _mmc3TmpVal = _mmc3IrqVal;
                        break;
                    case 0xE000:
                        _mmc3IrcOn = false;
                        _mmc3IrqVal = _mmc3TmpVal;
                        irqRaised = false;
                        updateIRQ();
                        
                        break; 
                    case 0xE001:
                        _mmc3IrcOn = true;
                        break;

                }
            }

            private void SwapChrBanks()
            {
                if (ppuBankSwap)
                {
                    CopyBanks(0, chr1kBank0 , 1);
                    CopyBanks(1, chr1kBank1 , 1);
                    CopyBanks(2, chr1kBank2 , 1);
                    CopyBanks(3, chr1kBank3 , 1);
                    CopyBanks(4, chr2kBank0 , 2);
                    CopyBanks(6, chr2kBank1 , 2);
                }
                else
                {
                    CopyBanks(4, chr1kBank0, 1);
                    CopyBanks(5, chr1kBank1, 1);
                    CopyBanks(6, chr1kBank2, 1);
                    CopyBanks(7, chr1kBank3, 1);
                    CopyBanks(0, chr2kBank0 , 2);
                    CopyBanks(2, chr2kBank1 , 2);
                }
            }

            private void SwapPrgRomBanks()
            {
        //|+-------- PRG ROM bank configuration (0: $8000-$9FFF swappable, $C000-$DFFF fixed to second-last bank;
        //|                                      1: $C000-$DFFF swappable, $8000-$9FFF fixed to second-last bank)

                if (prgSwap == 1)
                {

                    SetupBankStarts(PrgRomCount * 2 - 2, prgSwitch2, prgSwitch1, PrgRomCount * 2 - 1);
                } else 
                {
                    SetupBankStarts(prgSwitch1, prgSwitch2, PrgRomCount * 2 - 2, PrgRomCount * 2 - 1);
                }

            }

            public override void UpdateScanlineCounter()
            {
                if (scanlineCounter == -1) return;

                if (scanlineCounter == 0)
                {
                    scanlineCounter = _mmc3IrqVal;
                    //Writing $00 to $C000 will result in a single IRQ being generated on the next rising edge of PPU A12. 
                    //No more IRQs will be generated until $C000 is changed to a non-zero value, upon which the 
                    // counter will start counting from the new value, generating an IRQ once it reaches zero. 
                    if (_mmc3IrqVal == 0)
                    {
                       irqRaised = true;
                       updateIRQ();
                        scanlineCounter = -1;
                    }
                }

                if (_mmc3TmpVal != 0)
                {
                    scanlineCounter = _mmc3TmpVal;
                    _mmc3TmpVal = 0;
                }
                else
                {
                    scanlineCounter = (scanlineCounter - 1) & 0xFF;
                }

                if (_mmc3IrcOn && scanlineCounter == 0)
                {
                    irqRaised = true;
                    updateIRQ();
                    // scanlineCounter = _mmc3IrqVal + 1;
                }
                
            }

            #endregion
        }

}
