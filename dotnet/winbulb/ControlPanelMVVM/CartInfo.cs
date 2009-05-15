using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine.Carts;

namespace WPFamicom.ControlPanelMVVM
{
    public class CartInfo
    {
        public string CartName { get; set; }
        public int NumberOfPrgRoms { get; set; }
        public int NumberOfChrRoms { get; set; }
        public int MapperID { get; set; }
        public NameTableMirroring Mirroring { get; set; }
        public string RomInfoString { get; set; }
    }
}
