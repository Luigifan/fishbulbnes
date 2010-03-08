using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using System.ComponentModel;
using NES.CPU.Fastendo.Hacking;
using fishbulbcommonui;

namespace Fishbulb.Common.UI
{


    public class CheatPanelVM : BaseNESViewModel
    {
        public CheatPanelVM()
        {
            Commands.Add("AddCheat", new InstigatorCommand(
                new CommandExecuteHandler(o => AddGenieCode()),
                new CommandCanExecuteHandler(o => CanAddGenieCode)
                ));
            Commands.Add("ClearCheats", new InstigatorCommand(
                new CommandExecuteHandler(o => ClearGenieCodes()),
                new CommandCanExecuteHandler(o => CanClearGenieCodes)
                ));
        }

        public class CheatVM : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public IMemoryPatch Patch { get; set; }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
            public void NotifyPropertyChanged(string propName)
            {
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs( propName));
            }

            public override string ToString()
            {
                return string.Format("{0} - {1}", Name, Patch.ToString());
            }

        }
        
        protected override void  OnAttachTarget()
        {


            gameGenieCodes = new List<string>();
            NotifyPropertyChanged("GameGenieCodes");

        }

        private List<CheatVM> cheats = new List<CheatVM>();

        public List<CheatVM> Cheats
        {
            get { return cheats; }
            set { cheats = value; }
        }

        private List<string> gameGenieCodes = new List<string>();

        public List<string> GameGenieCodes
        {
            get { return gameGenieCodes; }
            set { gameGenieCodes = value; }
        }

        public bool CanClearGenieCodes
        {
            get { return TargetMachine != null; }
        }

        public void ClearGenieCodes()
        {
            cheats.RemoveAll(p => gameGenieCodes.Contains(p.Name));
            TargetMachine.ClearGenieCodes();
            NotifyPropertyChanged("GameGenieCodes");
            NotifyPropertyChanged("Cheats");
        }

        string _currentCode = string.Empty;
        public string CurrentCode
        {
            get
            {
                return _currentCode;
            }
            set
            {

                    _currentCode = value;
                    NotifyPropertyChanged("CanAddGenieCode");
                    NotifyPropertyChanged("Commands");
                
            }
        }

        public bool CanAddGenieCode
        {
            get { return TargetMachine != null ; }
        }

        public bool Cheating
        {
            get
            {
                return (TargetMachine == null) ? false : TargetMachine.Cpu.Cheating;
            }
            set
            {
                if (TargetMachine == null) return;
                TargetMachine.Cpu.Cheating = value;
            }
        }


        public void AddGenieCode()
        {
            IMemoryPatch patch = null;
            if (TargetMachine.AddGameGenieCode(_currentCode, out patch))
            {
                gameGenieCodes.Add(_currentCode);
                cheats.Add(new CheatVM() { Name = _currentCode, Patch = patch });
                NotifyPropertyChanged("GameGenieCodes");
                NotifyPropertyChanged("Cheats");
            }
        }



        public override string CurrentView
        {
            get { return "CheatPanel"; }
        }


        public override string CurrentRegion
        {
            get { return "controlPanel.3"; }
        }

        public override string Header
        {
            get { return "Cheat Panel"; }
        }


    }
}
