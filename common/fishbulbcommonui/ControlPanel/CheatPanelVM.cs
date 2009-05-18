using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using System.ComponentModel;

namespace fishbulbcommonui.ControlPanel
{
    public class CheatPanelVM : IProfileViewModel
    {
        
        private NESMachine nes = null;

        public CheatPanelVM(NESMachine nes)
        {
            this.nes = nes;
            commands.Add("AddCheat",new InstigatorCommand(
                new CommandExecuteHandler(o => AddGenieCode()),
                new CommandCanExecuteHandler(o => CanAddGenieCode)
                ));
        }

        private List<string> gameGenieCodes = new List<string>();

        public IEnumerable<string> GameGenieCodes
        {
            get { return gameGenieCodes; }
            // set { gameGenieCodes = value; }
        }

        public bool CanClearGenieCodes
        {
            get { return nes != null; }
        }

        public void ClearGenieCodes()
        {
            nes.ClearGenieCodes();
            gameGenieCodes = new List<string>();
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
                }
            }
        }

        public bool CanAddGenieCode
        {
            get { return nes != null && CurrentCode.Length == 6 || CurrentCode.Length == 8; }
        }

        public void AddGenieCode()
        {
            if (nes.AddGameGenieCode(CurrentCode))
            {
                gameGenieCodes.Add(CurrentCode);
            }
        }

        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string parameter)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(parameter));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IProfileViewModel Members

        public string CurrentView
        {
            get { return "CheatPanel"; }
        }
        
        Dictionary<string, ICommandWrapper> commands = new Dictionary<string, ICommandWrapper>() ;

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return Commands; }
        }

        public IEnumerable<IProfileViewModel> ChildViewModels
        {
            get { return new List<IProfileViewModel>(); }
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
