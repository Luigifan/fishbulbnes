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

namespace Fishbulb.Common.UI
{


    public class CheatPanelVM : IViewModel
    {
        public class CheatVM : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public IMemoryPatch Patch { get; set; }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;
            #endregion

            public override string ToString()
            {
                return string.Format("{0} - {1}", Name, Patch.ToString());
            }

        }

        
        private NESMachine nes = null;

        public CheatPanelVM(NESMachine nes)
        {
            this.nes = nes;
            commands.Add("AddCheat",new InstigatorCommand(
                new CommandExecuteHandler(o => AddGenieCode() ),
                new CommandCanExecuteHandler(o => CanAddGenieCode)
                ));
            commands.Add("ClearCheats", new InstigatorCommand(
                new CommandExecuteHandler(o => ClearGenieCodes()),
                new CommandCanExecuteHandler(o => CanClearGenieCodes)
                ));

            gameGenieCodes = new List<string>();
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
            get { return nes != null; }
        }

        public void ClearGenieCodes()
        {
            cheats.RemoveAll(p => gameGenieCodes.Contains(p.Name));
            nes.ClearGenieCodes();
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
                if (_currentCode != value)
                {
                    _currentCode = value;
                    NotifyPropertyChanged("CanAddGenieCode");
                    NotifyPropertyChanged("Commands");
                }
            }
        }

        public bool CanAddGenieCode
        {
            get { return nes != null && CurrentCode.Length == 6 || CurrentCode.Length == 8; }
        }

        public bool Cheating
        {
            get
            {
                return (nes == null) ? false : nes.Cpu.Cheating;
            }
            set
            {
                if (nes == null) return;
                nes.Cpu.Cheating = value;
            }
        }


        public void AddGenieCode()
        {
            IMemoryPatch patch = null;
            if (nes.AddGameGenieCode(_currentCode, out patch))
            {
                gameGenieCodes.Add(_currentCode);
                cheats.Add(new CheatVM() { Name = _currentCode, Patch = patch });
                NotifyPropertyChanged("GameGenieCodes");
                NotifyPropertyChanged("Cheats");
            }
        }

        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string parameter)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(parameter));
            OnPropertyChanged(parameter);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string PropName) { }

        #endregion

        #region IProfileViewModel Members

        public string CurrentView
        {
            get { return "CheatPanel"; }
        }
        
        Dictionary<string, ICommandWrapper> commands = new Dictionary<string, ICommandWrapper>() ;

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return commands; }
        }

        public IEnumerable<IViewModel> ChildViewModels
        {
            get { return new List<IViewModel>(); }
        }

        public string CurrentRegion
        {
            get { return "controlPanel.3"; }
        }

        public string Header
        {
            get { return "Cheat Panel"; }
        }

        public object DataModel
        {
            get { return null; }
        }

        #endregion
    }
}
