using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine;

namespace SlimDXBindings
{
    public class SlimDXZapper : IControlPad, IPixelAwareDevice
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
                //if (NextControlByteSet != null) NextControlByteSet(this, new ControlByteEventArgs((byte)currByte));
            }
        }

        int pixelICareAbout;
        int pixel;


        public void SetPixel(int pixel)
        {
            pixelICareAbout = pixel;
        }

        /// <summary>
        /// called when the mouse is clicked
        /// </summary>
        /// <param name="luma"></param>
        public void TriggerDown()
        {
            CurrentByte &= ~16;
        }

        public void TriggerUp()
        {
            CurrentByte |= 16;
            // pixelICareAbout = -1;
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


        public int GetByte(int clock)
        {
            //if ((currByte & 16) == 16)
            //    Console.WriteLine("Hit");
            //else
            //    Console.WriteLine("Miss");
            if (NeedPixelNow != null) NeedPixelNow(this, new ClockedRequestEventArgs() { Clock = clock } );
            return currByte;
        }

        public void SetByte(int clock, int data)
        {
            
        }

        #endregion

        #region IControlPad Members




        public int PixelICareAbout
        {
            get { return pixelICareAbout; }
        }

        public int PixelValue
        {
            set 
            {
                
                pixel = value ;
                // luma peaks at 82
                if (pixel > 60)
                {
                    currByte &= ~8;
                }
                else
                {
                    currByte |= 8;
                }
            }
        }

        #endregion

        #region IPixelAwareDevice Members


        public event EventHandler<ClockedRequestEventArgs> NeedPixelNow;

        #endregion

        #region IControlPad Members

        #endregion
    }
}
