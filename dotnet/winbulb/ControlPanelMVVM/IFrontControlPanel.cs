using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NES.Machine.ControlPanel;
using System.Collections.ObjectModel;
using NES.CPU.nitenedo;

namespace InstiBulb.ControlPanelMVVM
{
    public interface IFrontControlPanel : INotifyPropertyChanged, IDisposable
    {
        // events sent to the machine

        void InsertCart(string fileName);

        string CurrentCartName { get; }

        RunningStatuses RunState { get; }

        void PowerOn();

        void PowerOff();

        void Reset();

        void RemoveCart();

        bool Paused { get; set; }



        NESMachine Target { get; }

        void Debug(bool value);



        CartInfo CartInfo { get; }
    }

}
