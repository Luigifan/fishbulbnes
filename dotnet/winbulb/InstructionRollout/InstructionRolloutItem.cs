using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using System.ComponentModel;

namespace WPFamicom.InstructionRollout
{
    public class InstructionRolloutItem : INotifyPropertyChanged
    {
        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private CPU2A03.Instruction _instruction;

        public CPU2A03.Instruction Instruction
        {
            get { return _instruction; }
            set { _instruction = value; }
        }

        private bool _hasBreakpoint;

        public bool HasBreakpoint
        {
            get { return _hasBreakpoint; }
            set {
                if (value != _hasBreakpoint)
                {
                    _hasBreakpoint = value;
                    NotifyPropertyChanged("HasBreakpoint");
                }
            }
        }

        public string Disassembly
        {
            get { return _instruction.Disassemble(); }
        }

        public override string ToString()
        {
            return string.Format("{0:x4) {1}", this.Address, this.Instruction.Disassemble());
        }

        #region INotifyPropertyChanged Members

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
