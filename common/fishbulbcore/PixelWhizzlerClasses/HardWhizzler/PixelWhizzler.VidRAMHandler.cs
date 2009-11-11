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

        //private byte[] _vidRAM = new byte[0x4000];

        //public byte[] VidRAM
        //{
        //    get { return _vidRAM; }
        //    set { _vidRAM = value; }
        //}

        //public byte[] cartCopyVidRAM
        //{
        //    get { return _vidRAM; }
        //    set { _vidRAM = value; }
        //}
    }
}
