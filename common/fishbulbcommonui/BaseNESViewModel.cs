using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;

namespace fishbulbcommonui
{
    public abstract class BaseNESViewModel : IViewModel
    {
        public abstract string CurrentView
        {
            get;
        }

        private Dictionary<string, ICommandWrapper> _commands = new Dictionary<string,ICommandWrapper>();

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return _commands; }
        }

        public virtual IEnumerable<IViewModel> ChildViewModels
        {
            get { return null; }
        }

        public abstract string CurrentRegion
        {
            get;
        }

        public abstract string Header
        {
            get;
        }

        private NESMachine _nesMachine;

        protected virtual void OnAttachTarget()
        {

        }

        public NESMachine TargetMachine
        {
            get { return _nesMachine; }
            set {  
                _nesMachine = value;
                OnAttachTarget();
                NotifyPropertyChanged("TargetMachine");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
                OnPropertyChanged(propName);
            }
        }

        protected virtual void OnPropertyChanged(string propName)
        {
        }
    }
}
