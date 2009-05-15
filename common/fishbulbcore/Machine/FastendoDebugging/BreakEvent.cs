using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.CPUDebugging;

namespace NES.CPU.Machine.FastendoDebugging
{
    public class BreakEventArgs : EventArgs
    {
        bool isError;

        public bool IsError
        {
            get { return isError; }
            set { isError = value; }
        }

        CPUBreakpoint breakpoint;

        public CPUBreakpoint Breakpoint
        {
            get { return breakpoint; }
            set { breakpoint = value; }
        }
    }
}
