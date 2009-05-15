using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.PortQueueing
{
    class PortWriteEntry
    {
        public PortWriteEntry(int time, ushort address, byte data)
        {
            this.time = time;
            this.address = address;
            this.data = data;
        }

        public int time;
        public ushort address;
        public byte data;
    }

    class QueuedPort : Queue<PortWriteEntry>
    {
        public QueuedPort() : base(256)
        {
        }
    }
}
