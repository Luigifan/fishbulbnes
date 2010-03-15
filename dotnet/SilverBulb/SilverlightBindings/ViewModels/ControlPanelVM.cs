
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.ROMLoader;
using fishbulbcommonui;
using System.IO;
using FishBulb;

namespace Fishbulb.Common.UI
{

    public class SilverlightControlPanelVM : BaseNESViewModel
    {


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


        public SilverlightControlPanelVM(IPlatformDelegates delegates ) : base(delegates)
        {
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
            Commands.Add("PauseToggle",
                new InstigatorCommand(new CommandExecuteHandler(o =>
                    PauseToggle()
                    ),
                    new CommandCanExecuteHandler(o => true)));
                    }

        void BrowseFile(object o)
        {
            if (TargetMachine != null)
            TargetMachine.SilverlightStart();

        }

        protected override void OnAttachTarget()
        {
            TargetMachine.RunStatusChangedEvent += TargetMachine_RunStatusChangedEvent;
        }

        protected override void OnDetachTarget()
        {
            TargetMachine.RunStatusChangedEvent -= TargetMachine_RunStatusChangedEvent;
        }


        void TargetMachine_RunStatusChangedEvent(object sender, EventArgs e)
        {
            NotifyPropertyChanged("PowerStatusText");
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
                switch (TargetMachine.RunState)
                {
                    case NES.Machine.ControlPanel.RunningStatuses.Unloaded:
                        return "";
                    case NES.Machine.ControlPanel.RunningStatuses.Off:
                        return "off";
                    case NES.Machine.ControlPanel.RunningStatuses.Paused:
                        return "paused";
                    case NES.Machine.ControlPanel.RunningStatuses.Running:
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


        void InsertCart(string fileName)
        {
            if (TargetMachine.IsRunning) PowerOff();

            Stream s = PlatformDelegates.LoadFile(fileName);
            if (s != null)
            {
                using (s)
                    TargetMachine.LoadCart(s);
            }

            UpdateCartInfo();
        }

        protected void UpdateCartInfo()
        {
            if (TargetMachine.Cart != null)
            {

                this.CartInfo = new CartInfo()
                {
                    CartName = TargetMachine.CurrentCartName,
                    MapperID = TargetMachine.Cart.MapperID,
                    Mirroring = TargetMachine.Cart.Mirroring,
                    RomInfoString = string.Format("Prg Rom Count: {0}, Chr Rom Count: {1}", TargetMachine.Cart.NumberOfPrgRoms, TargetMachine.Cart.NumberOfChrRoms)
                };

                NotifyPropertyChanged("CurrentCartName");
                NotifyPropertyChanged("CartInfo");
            }
        }



        void PowerOn()
        {
            TargetMachine.IsDebugging = false;

            switch (TargetMachine.RunState)
            {
                case NES.Machine.ControlPanel.RunningStatuses.Running:
                    break;

                default:
                    TargetMachine.Paused = false;
                    TargetMachine.Reset();
                    TargetMachine.ThreadRuntendo();
                    break;
            }

        }

        void PowerToggle()
        {

            

            if (TargetMachine.RunState == NES.Machine.ControlPanel.RunningStatuses.Running)
            {
                PowerOff();
            }
            else
            {
                PowerOn();
            }
        }

        void PauseToggle()
        {
            Paused = !Paused;
        }

        void PowerOff()
        {
            if (TargetMachine.IsRunning)
                TargetMachine.PowerOff();
        }

        
        public bool Paused
        {
            get { return TargetMachine.Paused; }
            set {
                if (TargetMachine != null && TargetMachine.Cart != null)
                {
                    TargetMachine.IsDebugging = false;

                    TargetMachine.Paused = value;
                }
                
            }
        }

    }
}
