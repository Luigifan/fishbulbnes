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

    public interface IControlPad : IDisposable
    {
        int CurrentByte { get; set; }
        void Refresh();

        int GetByte();
        void SetByte(int data);

        event EventHandler<ControlByteEventArgs> NextControlByteSet;
    }
}
