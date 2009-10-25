using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine.Carts;

namespace NES.CPU.PPUClasses
{
    public partial class PixelWhizzler
    {
        INESCart chrRomHandler;

        private byte[] _vidRAM = new byte[0x4000];

        public byte[] VidRAM
        {
            get { return _vidRAM; }
            set { _vidRAM = value; }
        }

        public byte[] cartCopyVidRAM
        {
            get { return _vidRAM; }
            set { _vidRAM = value; }
        }
    }
}
