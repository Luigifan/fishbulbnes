using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.CPUDebugging
{
    public struct CPUBreakpoint
    {
        int address;

        public int Address
        {
            get { return address; }
            set { address = value; }
        }

        public override string ToString()
        {
            return string.Format("{0:x4}", address);
        }

        public override bool Equals(object obj)
        {
            return (this.address == ((CPUBreakpoint)obj).address);
        }

        public override int GetHashCode()
        {
            return address;
        }
    }
}
