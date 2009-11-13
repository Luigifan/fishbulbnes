using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine.Carts;

namespace NES.CPU.PPUClasses
{
    public partial class HardWhizzler
    {
        INESCart chrRomHandler;

        public INESCart ChrRomHandler
        {
            get { return chrRomHandler; }
            set { chrRomHandler = value; }
        }
    }
}
