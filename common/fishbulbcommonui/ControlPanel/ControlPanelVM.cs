
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;

namespace Fishbulb.Common.UI
{

    public delegate void CommandExecuteHandler(object parm);
    public delegate bool CommandCanExecuteHandler(object parm);

    public enum RunningStatuses
    {
        Unloaded,
        Off,
        Running,
        Paused
    }

    public class InstigatorCommand : ICommandWrapper
    {
        CommandExecuteHandler exectutor;
        CommandCanExecuteHandler canExecutor;
        public InstigatorCommand(CommandExecuteHandler exectutor, CommandCanExecuteHandler canExecutor)
        {
            this.exectutor = exectutor;
            this.canExecutor = canExecutor;
        }

        #region ICommandWrapper Members

        public void Execute(object param)
        {
            exectutor(param);
        }

        public bool CanExecute(object param)
        {
            return canExecutor(param);
        }

        #endregion
    }

    public class ControlPanelVM : IProfileViewModel
    {

        #region IProfileViewModel implementation
        public string CurrentView
        {
            get
            {
                return "FrontPanel";
            }
        }

        Dictionary<string, ICommandWrapper> commands = new Dictionary<string, ICommandWrapper>();

        public Dictionary<string, ICommandWrapper> Commands
        {
            get
            {
                return commands;
            }
        }

        public IEnumerable<IProfileViewModel> ChildViewModels
        {
            get
            {
                return new IProfileViewModel[0];
            }
        }

        public string CurrentRegion
        {
            get
            {
                return "controlPanel.0";
            }
        }

        public string Header
        {
            get
            {
                return null;
            }
        }

        public object DataModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        NESMachine _target;

        public ControlPanelVM(NESMachine machine)
        {
            _target = machine;

            commands.Add("LoadRom",
                new InstigatorCommand(new CommandExecuteHandler(o => InsertCart(o as string)),
                    new CommandCanExecuteHandler(CanInsertCart)));
            commands.Add("PowerToggle",
                new InstigatorCommand(new CommandExecuteHandler(o => PowerOn()),
                    new CommandCanExecuteHandler(o => true)));
            runstate = RunningStatuses.Unloaded;
        }

        bool CanInsertCart(object o)
        {
            return true;

        }

        public string CurrentCartName
        {
            get
            {
                if (_target != null)
                {
                    if (string.IsNullOrEmpty(_target.CurrentCartName))
                    {
                        return "Load Game";
                    }
                    return _target.CurrentCartName;
                }
                return "Load Game";
            }
        }

        public string PowerStatusText
        {
            get
            {
                switch (runstate)
                {
                    case RunningStatuses.Unloaded:
                    case RunningStatuses.Off:
                        return "off";
                    default:
                        return "on";
                }
            }
        }

        CartInfo _cartInfo;

        public CartInfo CartInfo
        {
            get { return _cartInfo; }
            set { _cartInfo = value; }
        }

        RunningStatuses runstate = RunningStatuses.Unloaded;

        public RunningStatuses RunState
        {
            get
            {
                return runstate;
            }
        }

        void InsertCart(string fileName)
        {
            _target.GoTendo(fileName);

            this.CartInfo = new CartInfo()
            {
                CartName = _target.CurrentCartName,
                MapperID = _target.Cart.MapperID,
                Mirroring = _target.Cart.Mirroring,
                RomInfoString = string.Format("Prg Rom Count: {0}, Chr Rom Count: {1}", _target.Cart.NumberOfPrgRoms, _target.Cart.NumberOfChrRoms)
            };

            runstate = RunningStatuses.Off;
            NotifyPropertyChanged("CurrentCartName");
            NotifyPropertyChanged("PowerStatusText");
            NotifyPropertyChanged("CartInfo");
        }

        void PowerOn()
        {
            _target.ThreadRuntendo();
            runstate = RunningStatuses.Running;
            NotifyPropertyChanged("PowerStatusText");
        }
        public bool Paused
        {
            get { return _target.Paused; }
            set { _target.Paused = value; }
        }
    }
}
