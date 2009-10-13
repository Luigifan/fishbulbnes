using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using System.ComponentModel;

namespace Fishbulb.Common.UI
{

    public class CheatPanelVM : IViewModel
    {
        
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
            gameGenieCodes.Add("Test");

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
            nes.ClearGenieCodes();
            gameGenieCodes.Clear();
            NotifyPropertyChanged("GameGenieCodes");

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

        public void AddGenieCode()
        {

            if (nes.AddGameGenieCode(_currentCode))
            {
                gameGenieCodes.Add(_currentCode);
                NotifyPropertyChanged("GameGenieCodes");
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
