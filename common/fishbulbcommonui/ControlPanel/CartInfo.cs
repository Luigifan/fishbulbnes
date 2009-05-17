using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine.Carts;

namespace Fishbulb.Common.UI
{
    public class CartInfo
    {
        public string CartName { get; set; }
        public int NumberOfPrgRoms { get; set; }
        public int NumberOfChrRoms { get; set; }
        public int MapperID { get; set; }
        public NameTableMirroring Mirroring { get; set; }
        public string RomInfoString { get; set; }

        public override string ToString()
        {
            StringBuilder bld = new StringBuilder();
            bld.Append("Name: ")
                .AppendLine(CartName)
                .Append("PrgROM count: ")
                .AppendLine(NumberOfPrgRoms.ToString())
                .Append("ChrROM count: ")
                .AppendLine(NumberOfChrRoms.ToString())
                .Append("Mapper ID: ")
                .Append(MapperID)
                .Append(" Mirroring: ")
                .AppendLine(Mirroring.ToString());
            return bld.ToString();
            
        }
    }
}
