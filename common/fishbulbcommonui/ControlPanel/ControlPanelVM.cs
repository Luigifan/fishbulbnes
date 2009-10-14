
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.ROMLoader;

namespace Fishbulb.Common.UI
{

    public delegate void CommandExecuteHandler(object parm);
    public delegate bool CommandCanExecuteHandler(object parm);
    public delegate string GetFileDelegate(string defaultExt, string Filter);

    public enum RunningStatuses
    {
        Unloaded,
        Off,
        Running,
        Paused
    }

    public class RunStatusChangedEventArgs : EventArgs
    {
        public RunStatusChangedEventArgs(RunningStatuses oldState, RunningStatuses newState)
        {
            NewState = newState;
            OldState = oldState;
        }

        public RunningStatuses NewState
        {
            get;
            private set;
        }

        public RunningStatuses OldState
        {
            get;
            private set;
        }
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

    public class ControlPanelVM : IViewModel
    {

        public event EventHandler<RunStatusChangedEventArgs> RunStatusChangedEvent;

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

        public IEnumerable<IViewModel> ChildViewModels
        {
            get
            {
                return new IViewModel[0];
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
            get;
            set;
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

        public ControlPanelVM(NESMachine machine, GetFileDelegate fileGetter)
        {
            _target = machine;
            this.fileGetter = fileGetter;
            commands.Add("LoadRom",
                new InstigatorCommand(new CommandExecuteHandler(o => InsertCart(o as string)),
                    new CommandCanExecuteHandler(CanInsertCart)));
            commands.Add("PowerToggle",
                new InstigatorCommand(new CommandExecuteHandler(o => 
                    PowerToggle()
                    ),
                    new CommandCanExecuteHandler(o => true)));
            commands.Add("BrowseRom",
                new InstigatorCommand(new CommandExecuteHandler(BrowseFile), new CommandCanExecuteHandler(CanInsertCart)));
            runstate = RunningStatuses.Unloaded;
        }

        GetFileDelegate fileGetter;

        void BrowseFile(object o)
        {
            string filename = fileGetter("*.nes", "NES Games (*.nes, *.nsf, *.zip)|*.nes;*.nsf;*.zip");
            if (filename != null)
                InsertCart(filename);
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
                        return "";
                    case RunningStatuses.Off:
                        return "off";
                    case RunningStatuses.Paused:
                        return "paused";
                    case RunningStatuses.Running:
                        return "on";
                    default:
                        return "";
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
            if (_target.IsRunning) PowerOff();

            try
            {
                _target.GoTendo(fileName);
            }
            catch (CartLoadException )
            {
                return;
            }

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

        void OnRunStatusChanged(RunningStatuses oldState, RunningStatuses newState)
        {
            if (RunStatusChangedEvent != null)
                RunStatusChangedEvent(this, new RunStatusChangedEventArgs(oldState, newState));
        }


        void PowerOn()
        {
            RunningStatuses oldState = runstate;
            switch (runstate)
            {
                case RunningStatuses.Off:
                    _target.Reset();
                    _target.ThreadRuntendo();
                    break;
                case RunningStatuses.Paused:
                    Paused=false;
                    break;
                case RunningStatuses.Unloaded:
                    return;
            }

            runstate = RunningStatuses.Running;

            OnRunStatusChanged(oldState, runstate);
            NotifyPropertyChanged("PowerStatusText");
            
        }

        void PowerToggle()
        {
            if (runstate == RunningStatuses.Running)
            {
                PowerOff();
            }
            else
            {
                PowerOn();
            }
        }

        void PowerOff()
        {
            RunningStatuses oldState = runstate;

            if (_target.IsRunning)
                _target.KeepRunning = false;

            runstate = RunningStatuses.Off;
            NotifyPropertyChanged("PowerStatusText");
            OnRunStatusChanged(oldState, runstate);

        }

        RunningStatuses prePauseState;

        public bool Paused
        {
            get { return _target.Paused; }
            set {
                RunningStatuses oldState = runstate;
                _target.Paused = value;
                if (runstate == RunningStatuses.Paused)
                    runstate = prePauseState;
                else
                {
                    prePauseState = runstate;
                    runstate = RunningStatuses.Paused;
                }

                OnRunStatusChanged(oldState, runstate);
                
            }
        }
    }
}
