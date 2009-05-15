using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;

namespace NES.CPU.nitenedo
{
    public class InputHandler : IMemoryMappedIOElement, NES.CPU.Machine.IControlPad
    {
        private int currentByte;
        private int nextByte;
        private int readNumber=0;
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

        public void SetNextControlByte(int data)
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
