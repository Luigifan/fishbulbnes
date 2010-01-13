using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {


        // hscroll, vscroll represent the latches written to by 2005, 2006
        private int _hScroll = 0;
        private int _vScroll = 0;
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
