using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using System.ComponentModel;

namespace fishbulbcommonui.SaveStates
{
    public class SaveStateVM : BaseNESViewModel
    {
        #region IViewModel Members

        protected override void OnAttachTarget()
        {
            for (int i = 0; i < 10; ++i)
            {
                stateInUse.Add(i, false);
            }

            selectedItem = stateInUse.ToList()[0];

            Commands.Add("GetSnapshot",
                new InstigatorCommand(new CommandExecuteHandler(o => GetSnapshot(o)),
                new CommandCanExecuteHandler(CanGetSnapshot)));
            Commands.Add("SetSnapshot",
                new InstigatorCommand(new CommandExecuteHandler(o => SetSnapshot(o)),
                new CommandCanExecuteHandler(CanSetSnapshot)));
        }

        void GetSnapshot(object s)
        {
            var p = (KeyValuePair<int, bool>)s;
            int i = (int)p.Key;
            TargetMachine.GetState(i);
            stateInUse[i] = true;
            NotifyPropertyChanged("StatesList");
            NotifyPropertyChanged("Commands");
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
            set { 
                selectedItem = value;
                NotifyPropertyChanged("Commands");
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
            TargetMachine.SetState(i);
            NotifyPropertyChanged("StatesList");
            NotifyPropertyChanged("Commands");
        }

        bool CanSetSnapshot(object o)
        {
            var p = (KeyValuePair<int, bool>)o;
            return p.Value;
        }

        public override string CurrentView
        {
            get { return "SaveStateView"; }
        }

        public override string CurrentRegion
        {
            get { return "SaveState"; }
        }

        public override string Header
        {
            get { return "Save States"; }
        }

        #endregion


    }
}
