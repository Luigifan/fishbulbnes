using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.Carts
{
    public class CartDebugEvent
    {
        int clock;

        public int Clock
        {
            get { return clock; }
            set { clock = value; }
        }

        string eventType;

        public string EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", clock, eventType);
        }
    }
}
