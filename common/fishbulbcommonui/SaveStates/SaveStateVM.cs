using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using System.ComponentModel;

namespace fishbulbcommonui.SaveStates
{
    public class SaveStateVM : IViewModel
    {
        #region IViewModel Members

        readonly NESMachine nes;

        public SaveStateVM(NESMachine nes)
        {
            for (int i = 0; i < 10; ++i)
            {
                stateInUse.Add(i, false);
            }

            selectedItem = stateInUse.ToList()[0];

            this.nes = nes;

            _commands.Add("GetSnapshot",
                new InstigatorCommand(new CommandExecuteHandler(o => GetSnapshot(o)),
                new CommandCanExecuteHandler(CanGetSnapshot)));
            _commands.Add("SetSnapshot",
                new InstigatorCommand(new CommandExecuteHandler(o => SetSnapshot(o)),
                new CommandCanExecuteHandler(CanSetSnapshot)));
        }

        void GetSnapshot(object s)
        {
            var p = (KeyValuePair<int, bool>)s;
            int i = (int)p.Key;
            nes.GetState(i);
            stateInUse[i] = true;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("StatesList"));
                PropertyChanged(this, new PropertyChangedEventArgs("Commands"));
            }
        }

        bool CanGetSnapshot(object o)
        {
            return true;
        }

        Dictionary<int, bool> stateInUse = new Dictionary<int, bool>();

        KeyValuePair<int, bool> selectedItem ;

        public KeyValuePair<int, bool> SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Commands"));
                    }
                }
        }

        public Dictionary<int, bool> StateInUse
        {
            get { return stateInUse; }
            set { stateInUse = value; }
        }

        public List<int> AvailableStates
        {
            get { return stateInUse.Keys.ToList(); }
        }

        public List<KeyValuePair<int, bool>> StatesList
        {
            get { return stateInUse.ToList(); }
        }

        void SetSnapshot(object s)
        {
            var p = (KeyValuePair<int, bool>)s;
            int i = (int)p.Key;
            nes.SetState(i);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("StatesList"));
                PropertyChanged(this, new PropertyChangedEventArgs("Commands"));
            }
        }

        bool CanSetSnapshot(object o)
        {
            var p = (KeyValuePair<int, bool>)o;
            return p.Value;
        }

        public string CurrentView
        {
            get { return "SaveStateView"; }
        }

        Dictionary<string, ICommandWrapper> _commands = new Dictionary<string, ICommandWrapper>();
        
        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return _commands; }
        }

        public IEnumerable<IViewModel> ChildViewModels
        {
            get { return null; }
        }

        public string CurrentRegion
        {
            get { return "SaveState"; }
        }

        public string Header
        {
            get { return "Save States"; }
        }

        public object DataModel
        {
            get { return null; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
