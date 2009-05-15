using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {
//NT:	name table
//AT:	attribute/color table
//PT:	pattern table
//FV:	fine vertical scroll latch/counter
//FH:	fine horizontal scroll latch
//VT:	vertical tile index latch/counter
//HT:	horizontal tile index latch/counter
//V:	vertical name table selection latch/counter
//H:	horizontal name table selection latch/counter
//S:	playfield pattern table selection latch
//PAR:	picture address register (as named in patent document)
//AR:	tile attribute (palette select) value latch
///1:	first  write to 2005 or 2006 since reading 2002
///2:	second write to 2005 or 2006 since reading 2002        

        // internal ppu registers
        // 3 bits each
        int fineVerticalScroll;
        int fineHorizontalScroll;
        
        // 5 bits each
        // wraps at 30
        int verticalTileIndex;
        // wraps at 32
        int horizontalTileIndex;
        
        // 1 bit each
        int verticalNameTableSelectionLatch;
        int horizontalNameTableSelectionLatch;


        // increment counters during access to 2007
        // inc amount is 1 or 32
        private void IncPPUCounters(int incAmount)
        {
            horizontalTileIndex += incAmount;
            if (horizontalTileIndex > 31)
            {
                horizontalTileIndex -= 32;
                verticalTileIndex = verticalTileIndex++;
                if (verticalTileIndex > 29)
                {
                    verticalTileIndex = 0;
                    horizontalNameTableSelectionLatch++;
                    if (horizontalNameTableSelectionLatch > 1)
                    {
                        horizontalNameTableSelectionLatch = 0;
                        verticalNameTableSelectionLatch++;
                        if (verticalNameTableSelectionLatch > 1)
                        {
                            verticalNameTableSelectionLatch = 0;
                            fineVerticalScroll += 1;
                            if (fineVerticalScroll > 7)
                            {
                                fineVerticalScroll = 0;
                            }
                        }
                    }
                }
            }
        }

        // counters during rendering

        // 6 bit counter, horizontalTileIndex is daisy chained to horizontalNameTableSelection
        private void IncHorizScrollCounter()
        {
            horizontalTileIndex++;
            if (horizontalTileIndex > 31)
            {
                horizontalNameTableSelectionLatch++;
                if (horizontalNameTableSelectionLatch > 1)
                {
                    horizontalNameTableSelectionLatch = 0;
                }
            }
        }

        // 9 bit counter, vertical scroll is daisychained to verticalTileIndex, to verticalNameTableSelection
        private void IncVertScrollCounter()
        {
            fineVerticalScroll++;
            if (fineVerticalScroll > 7)
            {
                fineVerticalScroll = 0;
                verticalTileIndex++;
                if (verticalTileIndex > 29)
                {
                    verticalTileIndex = 0;
                    verticalNameTableSelectionLatch++;
                    if (verticalNameTableSelectionLatch > 1)
                    {
                        verticalNameTableSelectionLatch = 0;
                    }
                }
            }
        }

        // hscroll, vscroll represent the latches written to by 2005, 2006
        private int _hScroll = 0, HScroll2 = 0;
        private int _vScroll = 0, VScroll2 = 0;
        private int lockedHScroll = 0, lockedVScroll = 0;

        public int HScroll
        {
            get { return lockedHScroll; }
        }

        public int VScroll
        {
            get { return lockedVScroll; }
        }
    }
}
