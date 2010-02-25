using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using InstiBulb.WpfKeyboardInput;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using fishbulbcommonui;

namespace InstiBulb.WinViewModels
{
    public class BindKeyCommand : ICommand
    {
        readonly WpfKeyConfigVM pad;
        public BindKeyCommand(WpfKeyConfigVM c)
        {
            pad = c;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var par = parameter as NesKeyBinding;
            pad.SetBinding(par);
        }

        #endregion
    }

    public class WpfKeyConfigVM : BaseNESViewModel
    {
        private BindKeyCommand keyCommand;

        public BindKeyCommand KeyCommand
        {
            get { return keyCommand; }
        }


        protected override void OnAttachTarget()
        {
            
        }

        private WpfKeyboardInput.WpfKeyboardControlPad dataModel;

        public WpfKeyConfigVM(WpfKeyboardControlPad model)
        {
            dataModel = model;
            keyCommand = new BindKeyCommand(this);
            RefreshKeys();
            
        }

        internal void SetBinding(NesKeyBinding binding)
        {
            if (!dataModel.NesKeyBindings.ContainsKey(binding.Key))
            {
                dataModel.NesKeyBindings.Add(binding.Key, binding.BoundValue);
                RefreshKeys();
            }
            else
            {
                dataModel.NesKeyBindings.Remove(binding.Key);
                dataModel.NesKeyBindings.Add(binding.Key, binding.BoundValue);
                RefreshKeys();
            }
        }

        void RefreshKeys()
        {
            keyBindings.Clear();
            foreach (Key k in dataModel.NesKeyBindings.Keys)
            {
             
                keyBindings.Add(new NesKeyBinding() { Key = k, BoundValue = dataModel.NesKeyBindings[k] });
            }
        }

        ObservableCollection<NesKeyBinding> keyBindings = new ObservableCollection<NesKeyBinding>();
        public ObservableCollection<NesKeyBinding> KeyBindings
        {
            get { return keyBindings; }
            set { keyBindings = value; }
        }

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        public override string CurrentView
        {
            get { return "Key Config"; }
        }

        public override string CurrentRegion
        {
            get { return ""; }
        }

        public override string Header
        {
            get { return "Key Configuration"; }
        }
    }
}
