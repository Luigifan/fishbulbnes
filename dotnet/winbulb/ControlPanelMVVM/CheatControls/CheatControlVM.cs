using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMCommanding;
using System.Windows.Input;
using NES.CPU.nitenedo;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InstiBulb.ControlPanelMVVM.CheatControls
{
    public class CheatControlVM :INotifyPropertyChanged, ICommandSink
    {
        private CommandSink commandSink = new CommandSink();
        public static readonly RoutedCommand AddGenieCodeCommand = new RoutedCommand();
        public static readonly RoutedCommand ClearGenieCodesCommand = new RoutedCommand();

        private NESMachine nes = null;

        public CheatControlVM(NESMachine nes)
        {
            this.nes = nes;
            nes.RunStatusChangedEvent += new EventHandler<EventArgs>(nes_RunStatusChangedEvent);
            commandSink.RegisterCommand(AddGenieCodeCommand,
                                param => CanAddGenieCode,
                                param => AddGenieCode()
                                );
            commandSink.RegisterCommand(ClearGenieCodesCommand,
                                            param => CanClearGenieCodes,
                                            param => ClearGenieCodes()
                                            );
        }

        void nes_RunStatusChangedEvent(object sender, EventArgs e)
        {
            NotifyPropertyChanged("CanClearGenieCodes");
            NotifyPropertyChanged("CurrentCode");
            NotifyPropertyChanged("CanAddGenieCode");
        }

        private ObservableCollection<string> gameGenieCodes = new ObservableCollection<string>();

        public ObservableCollection<string> GameGenieCodes
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

        #region ICommandSink Members

        public bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            return commandSink.CanExecuteCommand(command, parameter, out handled);
        }

        public void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            commandSink.ExecuteCommand(command, parameter, out handled);
        }

        #endregion
    }
}
