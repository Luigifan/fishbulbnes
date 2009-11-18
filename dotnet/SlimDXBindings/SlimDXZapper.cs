using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine;

namespace SlimDXBindings
{
    public class SlimDXZapper : IControlPad
    {
        #region IControlPad Members

        int currByte;
        public int CurrentByte
        {
            get
            {
                return currByte;
            }
            set
            {
                currByte = value;
                if (NextControlByteSet != null) NextControlByteSet(this, new ControlByteEventArgs((byte)currByte));
            }
        }

        /// <summary>
        /// called when the mouse is clicked
        /// </summary>
        /// <param name="luma"></param>
        public void TriggerDown(float luma)
        {
            currByte |= 32;
            if (luma > 0.5)
                currByte |= 16;
            else
                currByte &= ~16;
        }

        public void TriggerUp()
        {
            currByte &= ~32;
        }

        public void Refresh()
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
