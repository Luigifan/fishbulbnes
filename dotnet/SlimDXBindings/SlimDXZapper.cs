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
        public void TriggerDown()
        {
            CurrentByte |= 16;
        }

        public void TriggerUp()
        {
            CurrentByte &= ~16;
        }

        public void SetLuma(byte b)
        {
            if (b > 175)
                currByte |= 8;
            else
                currByte &= ~8;
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

        #region IControlPad Members


        public int GetByte()
        {
            //if ((currByte & 16) == 16)
            //    Console.WriteLine("Hit");
            //else
            //    Console.WriteLine("Miss");

            return currByte;
        }

        public void SetByte(int data)
        {
            
        }

        #endregion
    }
}
