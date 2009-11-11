using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public partial class HardWhizzler
    {


        private Queue<PPUWriteEvent> events = new Queue<PPUWriteEvent>();

        public Queue<PPUWriteEvent> Events
        {
            get { return events; }
        }
        /// <summary>
        /// A class to encapsulate a PPU "event" for debugging/tracing purposes
        /// </summary>

    }
}
