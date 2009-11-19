using System;
namespace NES.CPU.Machine
{
    public class ControlByteEventArgs : EventArgs
    {
        byte nextValue;

        public ControlByteEventArgs(byte value)
        {
            this.nextValue = value;
        }

        public byte NextValue
        {
            get { return nextValue; }
            set { nextValue = value; }
        }
    }

    public class ClockedRequestEventArgs : EventArgs
    {
        public int Clock { get; set; }
    }

    public interface IPixelAwareDevice
    {
        int PixelICareAbout
        {
            get;
        }

        int PixelValue
        {
            set;
        }

        event EventHandler<ClockedRequestEventArgs> NeedPixelNow;
    }


    public interface IControlPad : IDisposable
    {
        int CurrentByte { get; set; }
        void Refresh();

        int GetByte(int clock);
        void SetByte(int clock, int data);



        event EventHandler<ControlByteEventArgs> NextControlByteSet;
    }
}
