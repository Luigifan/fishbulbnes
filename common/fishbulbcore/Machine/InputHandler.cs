using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using NES.CPU.Machine;

namespace NES.CPU.nitenedo
{
    public class InputHandler : IMemoryMappedIOElement
    {
        private int currentByte;
        private int nextByte;
        private int readNumber=0;

        private IControlPad controlPad;

        public IControlPad ControlPad
        {
            get { return controlPad; }
            set { controlPad = value;
            controlPad.NextControlByteSet += new EventHandler<ControlByteEventArgs>(controlPad_NextControlByteSet);
            }
        }

        void controlPad_NextControlByteSet(object sender, ControlByteEventArgs e)
        {
            SetNextControlByte(e.NextValue);
        }

        private object inputLock = new object();

        public int GetByte(int address)
        {
            int result = (currentByte >> readNumber) & 0x01;
            readNumber = (readNumber + 1) & 7;
            return (result | 0x40) & 0xFF;

        }

        public void SetByte(int address, int data)
        {
            if ((data & 1) == 1)
            {
                currentByte = nextByte;
                // if im pushing up, i cant be pushing down
                if ((currentByte & 16) == 16) currentByte = currentByte & ~32;
                // if im pushign left, i cant be pushing right.. seriously, the nes will glitch
                if ((currentByte & 64) == 64) currentByte = currentByte & ~128;

                readNumber = 0;
            }
            if (data == 0) // strobed this port, get the next byte
            {
            }
        }

        private void SetNextControlByte(int data)
        {

            nextByte = data;

        }

        // mainly for debugging purposes
        public int CurrentByte
        {
            get {

                return currentByte;
            }
            set
            {
                currentByte = value;
            }
        }



    }
}
