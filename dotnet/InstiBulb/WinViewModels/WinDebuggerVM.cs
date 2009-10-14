using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using System.Collections.ObjectModel;
using NES.CPU.PPUClasses;
using NES.CPU.nitenedo;

namespace InstiBulb.WinViewModels
{
    public class WinDebuggerVM : DebuggerVM
    {

        public WinDebuggerVM(NESMachine nes)
            : base(nes)
        {
            base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(WinDebuggerVM_PropertyChanged);
        }

        void WinDebuggerVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FrameWriteEvents")
            {
                _PPUWriteEvents.Clear();
                foreach (var p in FrameWriteEvents)
                {
                    _PPUWriteEvents.Add(p);
                }

            }
        }

        ObservableCollection<PPUWriteEvent> _PPUWriteEvents = new ObservableCollection<PPUWriteEvent>();
        public ObservableCollection<PPUWriteEvent> PPUWriteEvents
        {
            get { return _PPUWriteEvents; }
        }


    }
}
