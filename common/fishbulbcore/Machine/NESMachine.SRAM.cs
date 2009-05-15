using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.nitenedo
{
    public delegate void SRAMWriterDelegate(string RomID, byte[] SRAM);
    public delegate byte[] SRAMReaderDelegate(string RomID);
    
    public partial class NESMachine
    {
        public SRAMReaderDelegate SRAMReader { get; set; }
        public SRAMWriterDelegate SRAMWriter { get; set; }
    }

}
