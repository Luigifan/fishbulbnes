using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine
{
    /// <summary>
    /// plugs up a nes control port, when nothing else is using it
    /// </summary>
    public class NullControlPad : IControlPad
    {
        #region IControlPad Members

        public int CurrentByte
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public void Refresh()
        {
        }

        public int GetByte(int clock)
        {
            return 0;
        }

        public void SetByte(int clock, int data)
        {
        }

        public event EventHandler<ControlByteEventArgs> NextControlByteSet;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
