
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.ROMLoader;
using fishbulbcommonui;

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

    public class ControlPanelVM : BaseNESViewModel
    {

        public event EventHandler<RunStatusChangedEventArgs> RunStatusChangedEvent;

        public override string CurrentView
        {
            get
            {
                return "FrontPanel";
            }
        }

        public override string CurrentRegion
        {
            get
            {
                return "controlPanel.0";
            }
        }

        public override  string Header
        {
            get
            {
                return null;
            }
        }


        public ControlPanelVM(GetFileDelegate fileGetter)
        {
            this.fileGetter = fileGetter;
            Commands.Add("LoadRom",
                new InstigatorCommand(new CommandExecuteHandler(o => InsertCart(o as string)),
                    new CommandCanExecuteHandler(CanInsertCart)));
            Commands.Add("PowerToggle",
                new InstigatorCommand(new CommandExecuteHandler(o => 
                    PowerToggle()
                    ),
                    new CommandCanExecuteHandler(o => true)));
            Commands.Add("BrowseRom",
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
                if (TargetMachine != null)
                {
                    if (string.IsNullOrEmpty(TargetMachine.CurrentCartName))
                    {
                        return "Load Game";
                    }
                    return TargetMachine.CurrentCartName;
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
            if (TargetMachine.IsRunning) PowerOff();

            TargetMachine.GoTendo(fileName);


            this.CartInfo = new CartInfo()
            {
                CartName = TargetMachine.CurrentCartName,
                MapperID = TargetMachine.Cart.MapperID,
                Mirroring = TargetMachine.Cart.Mirroring,
                RomInfoString = string.Format("Prg Rom Count: {0}, Chr Rom Count: {1}", TargetMachine.Cart.NumberOfPrgRoms, TargetMachine.Cart.NumberOfChrRoms)
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
            TargetMachine.IsDebugging = false;
            RunningStatuses oldState = runstate;
            switch (runstate)
            {
                case RunningStatuses.Off:
                    TargetMachine.Reset();
                    TargetMachine.ThreadRuntendo();
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
            TargetMachine.IsDebugging = false;

            RunningStatuses oldState = runstate;

            if (TargetMachine.IsRunning)
                TargetMachine.ThreadStoptendo();

            runstate = RunningStatuses.Off;
            NotifyPropertyChanged("PowerStatusText");
            OnRunStatusChanged(oldState, runstate);

        }

        RunningStatuses prePauseState;

        public bool Paused
        {
            get { return TargetMachine.Paused; }
            set {
                TargetMachine.IsDebugging = false;

                RunningStatuses oldState = runstate;
                TargetMachine.Paused = value;
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
