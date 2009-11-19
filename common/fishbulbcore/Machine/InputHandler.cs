using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using NES.CPU.Machine;

namespace NES.CPU.nitenedo
{
    public class InputHandler : IClockedMemoryMappedIOElement
    {
        private int currentByte;
        private int nextByte;
        private int readNumber=0;

        private IControlPad controlPad;
        private bool isZapper = false;

        public bool IsZapper
        {
            get { return isZapper; }
            set { isZapper = value; }
        }
        public InputHandler()
        {
            controlPad = null;
        }

        public InputHandler(IControlPad padOne)
        {
            ControlPad = padOne;
        }

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

        public int GetByte(int clock, int address)
        {
            return controlPad.GetByte(clock);


        }

        public void SetByte(int clock, int address, int data)
        {
            controlPad.SetByte(clock, data);
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



        #region IClockedMemoryMappedIOElement Members


        public MachineEvent NMIHandler
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IRQAsserted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int NextEventAt
        {
            get { throw new NotImplementedException(); }
        }

        public void HandleEvent(int Clock)
        {
            throw new NotImplementedException();
        }

        public void ResetClock(int Clock)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
