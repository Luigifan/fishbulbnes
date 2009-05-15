using System;
namespace NES.CPU.Machine
{
    public interface IControlPad
    {
        int CurrentByte { get; set; }
        void SetNextControlByte(int data);
    }
}
