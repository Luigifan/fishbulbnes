using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {
        private byte[] cartChrRoms;
        private int vidRamStart = 0;

        public byte[] CartChrRoms
        {
            get { return cartChrRoms; }
            set { cartChrRoms = value; }
        }
        
        //TODO: implement
        private void SwapBanks()
        {
        }

    }
}
