using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using FishBulb;

namespace fishbulbcommonui
{
    public abstract class BaseNESViewModel : IViewModel, IDisposable
    {

        public BaseNESViewModel(IPlatformDelegates delegates)
        {
            platformDelegates = delegates;
        }

        public abstract string CurrentView
        {
            get;
        }

        IPlatformDelegates platformDelegates;

        public IPlatformDelegates PlatformDelegates
        {
            get { return platformDelegates; }
            set { platformDelegates = value; }
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

        protected virtual void OnDetachTarget()
        {

        }

        public NESMachine TargetMachine
        {
            get { return _nesMachine; }
            set {
                if (_nesMachine != null)
                    OnDetachTarget();

                _nesMachine = value;
                OnAttachTarget();
                NotifyPropertyChanged("TargetMachine");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

#if SILVERLIGHT
        System.Windows.Threading.Dispatcher dispatcher;

        public System.Windows.Threading.Dispatcher Dispatcher
        {
            get { return dispatcher; }
            set { dispatcher = value; }
        }


#endif
        protected virtual void NotifyPropertyChanged(string propName)
        {
#if SILVERLIGHT
            if (Dispatcher != null)
                Dispatcher.BeginInvoke(
                    new Action(
                        delegate 
                        {
                            if (PropertyChanged != null)
                                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName)); 
                        }
                    )
                );
#else
                if (PropertyChanged != null)
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
#endif
                OnPropertyChanged(propName);
        }

        protected virtual void OnPropertyChanged(string propName)
        {
        }

        public void Dispose()
        {
            if (_nesMachine != null)
                OnDetachTarget();
        }
    }
}
