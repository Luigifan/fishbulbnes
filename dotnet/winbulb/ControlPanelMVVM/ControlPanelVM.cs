using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.Machine.ControlPanel;
using VMCommanding;
using System.Windows.Input;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using WPFamicom.FileLoader;
using WPFamicom.Configuration;
using WPFamicom.ControlPanelMVVM.CheatControls;
using NES.CPU.nitenedo.Interaction;
using WPFamicom.GameDisplay;
using System.Windows.Markup;
using System.Windows.Controls;

namespace WPFamicom.ControlPanelMVVM
{

    public class ControlPanelVM : INotifyPropertyChanged, ICommandSink, IDisposable
    {
        private IFrontControlPanel _frontPanel;

        private IFileLoader _stateLoader;
        private IFileSaver _stateSaver;

        CheatControlVM cheatController;

        DisplayContextSwitcher contextSwitcher;

        public DisplayContextSwitcher ContextSwitcher
        {
            get { return contextSwitcher; }
        }

        public CheatControlVM CheatController
        {
            get { return cheatController; }
        }

        public IFrontControlPanel FrontPanel
        {
            get { return _frontPanel; }
            set 
            {
                // if we swap in a new control panel, 
                // make sure to unwire any old events
                if (_frontPanel != value)
                {
                    if (_frontPanel != null)
                    {
                        UnregisterCommands();
                    }
                    _frontPanel = value;
                    RegisterCommands();
                }
            }
        }

        IDisplayContext _viewer;

        NESDisplay display;
        public bool Paused
        {
            get
            {
                return FrontPanel.Paused;
            }

            set
            {
                FrontPanel.Paused = value;
                _viewer.SetPausedState(value);
            }
        }

        ControlPanel uiPanel;

        public ControlPanelVM(IFrontControlPanel panel, NESDisplay display,  IFileLoader stateLoader, IFileSaver stateSaver, CheatControlVM cheatController
            , ControlPanel uiPanel
            )
        {
            this.uiPanel = uiPanel;
            this.cheatController = cheatController;

            contextSwitcher = new DisplayContextSwitcher(display);
            contextSwitcher.PropertyChanged += new PropertyChangedEventHandler(contextSwitcher_PropertyChanged);
            contextSwitcher.CurrentDisplayContext = DisplayContexts.OpenGL;
            this.display = display;
            _viewer = display.Context;
            for (int i = 0; i < 10; ++i)
            {
                availableStates[i] = i;
            }

            _stateLoader = stateLoader;
            _stateSaver = stateSaver;
            _frontPanel = panel;
            RegisterCommands();
        }

        void contextSwitcher_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (display != null)
                _viewer = display.Context;
            try
            {
                if (display.Context.PropertiesPanel == null)
                {
                    uiPanel.renderPropertiesHolder.Child = null;
                }
                else
                {

                    uiPanel.renderPropertiesHolder.Child = (UserControl)XamlReader.Parse(display.Context.PropertiesPanel);
                    uiPanel.renderPropertiesHolder.DataContext = display.Context;
                    uiPanel.Focus();
                }
            }
            catch
            {
             //   Console.WriteLine("Fail");
            }
        }

        private void UnregisterCommands()
        {
            commandSink.UnregisterCommand(PowerOnCommand);
            commandSink.UnregisterCommand(PowerOffCommand);
            commandSink.UnregisterCommand(ResetCommand);
            commandSink.UnregisterCommand(InsertCartCommand);
            commandSink.UnregisterCommand(RemoveCartCommand);
        }

        private void RegisterCommands()
        {
            if (_frontPanel == null) return;
            commandSink.RegisterCommand(PowerOnCommand,
                                            param => CanPowerOn,
                                            param => _frontPanel.PowerOn()
                                            );
            commandSink.RegisterCommand(PowerOffCommand,
                                            param => CanPowerOff,
                                            param => _frontPanel.PowerOff()
                                            );
            commandSink.RegisterCommand(ResetCommand,
                                            param => CanReset,
                                            param => _frontPanel.Reset()
                                            );
            commandSink.RegisterCommand(InsertCartCommand,
                                            param => CanInsertCart,
                                            param => InsertCart()
                                            );
            commandSink.RegisterCommand(RemoveCartCommand,
                                            param => CanRemoveCart,
                                            param => _frontPanel.RemoveCart()
                                            );


            commandSink.RegisterCommand(GetSnapshotCommand, 
                                            param => CanGetSnapshot,
                                            param => GetSnapshot()
                                            );
            commandSink.RegisterCommand(SetSnapshotCommand,
                                            param => CanSetSnapshot,
                                            param => SetSnapshot()
                                            );
            commandSink.RegisterCommand(SaveSnapshotCommand,
                                            param => CanSaveSnapshot,
                                            param => SaveSnapshot()
                                            );
            commandSink.RegisterCommand(LoadSnapshotCommand,
                                            param => CanLoadSnapshot,
                                            param => LoadSnapshot()
                                            );



            commandSink.RegisterCommand(ShowGameInfoCommand,
                                        param => _frontPanel.RunState != RunningStatuses.Off,
                                        param => this.ShowCartInfoPopUp());

        }

        readonly CommandSink commandSink = new CommandSink();

        public CommandSink CommandSink
        {
            get { return commandSink; }
        } 

        #region Commands

        public static readonly RoutedCommand PowerOnCommand = new RoutedCommand();
        public static readonly RoutedCommand PowerOffCommand = new RoutedCommand();
        public static readonly RoutedCommand ResetCommand = new RoutedCommand();

        public static readonly RoutedCommand InsertCartCommand = new RoutedCommand();
        public static readonly RoutedCommand RemoveCartCommand = new RoutedCommand();

        public static readonly RoutedCommand GetSnapshotCommand = new RoutedCommand();
        public static readonly RoutedCommand SetSnapshotCommand = new RoutedCommand();

        public static readonly RoutedCommand SaveSnapshotCommand = new RoutedCommand();
        public static readonly RoutedCommand LoadSnapshotCommand = new RoutedCommand();


        public static readonly RoutedCommand ShowGameInfoCommand = new RoutedCommand();

        #endregion

        public bool CanInsertCart
        {
            get
            {
                if (_frontPanel != null)
                {
                    return _frontPanel.RunState == RunningStatuses.Unloaded;
                }

                return true;
            }
        }

        public void InsertCart()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = NESConfigManager.LastROMFolder;
            dlg.DefaultExt = "*.nes"; // Default file extension
            dlg.Filter = "NES Games (*.nes, *.nsf, *.zip)|*.nes;*.nsf;*.zip"; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                _frontPanel.InsertCart(filename);
                NESConfigManager.LastROMFolder = 
                    Directory.GetParent(dlg.FileName).FullName;

            }
            contextSwitcher.UpdateDisplayContext();

        }

        public bool CanPowerOn
        {
            get
            {
                if (_frontPanel != null)
                {
                    return _frontPanel.RunState != RunningStatuses.Unloaded;
                }

                return false;
            }
        }

        public bool CanPowerOff
        {

            get
            {
                if (_frontPanel != null)
                {
                    return (_frontPanel.RunState == RunningStatuses.Paused || _frontPanel.RunState == RunningStatuses.Running);
                }

                return false;
            }
        }

        public bool CanReset
        {
            get
            {
                if (_frontPanel != null)
                {
                    //  !off && !unloaded
                    return _frontPanel.RunState != RunningStatuses.Off && _frontPanel.RunState != RunningStatuses.Unloaded;
                }
                return false;
            }
        }

        public bool CanRemoveCart
        {
            get
            {
                if (_frontPanel != null)
                {
                    return _frontPanel.RunState != RunningStatuses.Unloaded;
                }

                return false;
            }
        }

        int currentState;

        public int CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        int[] availableStates = new int[10];
        
        public int[] AvailableStates
        {
            get
            {
                return availableStates;
            }
        }

        public bool CanGetSnapshot
        {
            get { return true; }
        }

        public void GetSnapshot()
        {
            _frontPanel.Target.GetState(currentState);
        }

        public bool CanSetSnapshot
        {
            get { return _frontPanel.Target.HasState(currentState); }
        }

        public void SetSnapshot()
        {
            _frontPanel.Target.SetState(currentState);
        }


        public bool CanSaveSnapshot
        {
            get { return _frontPanel.Target.HasState(currentState); }
        }

        public void SaveSnapshot()
        {
            using (BinaryWriter writer = _stateSaver.BrowseForFile())
            {
                _frontPanel.Target.WriteState(currentState, writer);
            }
        }

        public bool CanLoadSnapshot
        {
            get { return _frontPanel.Target.RunState == RunningStatuses.Paused || _frontPanel.Target.RunState == RunningStatuses.Running; }
        }

        public void LoadSnapshot()
        {
            using (BinaryReader reader = _stateLoader.BrowseForFile())
            {
                _frontPanel.Target.ReadState(currentState, reader);
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public string GameName
        {
            get
            {
                return _frontPanel.CurrentCartName;
            }
        }

        private bool _isDebugging;

        public bool IsDebugging
        {
            get 
            {
                return _isDebugging;
            }
            set
            {
                if (value != _isDebugging)
                {
                    _isDebugging = value;
                    FrontPanel.Debug(value);
                    NotifyPropertyChanged("DebuggerIsVisible");
                    NotifyPropertyChanged("PPUDebuggerIsVisible");

                }
            }
        }

        private DebuggerVM debugger;
        private Debugger _debuggerWindow = null;
        private PPUViewer _ppuViewer = null;


        private bool _debuggerIsVisible;

        public bool DebuggerIsVisible
        {
            get { return _debuggerIsVisible; }
            set {
                _debuggerIsVisible = value;
                if (_debuggerIsVisible)
                {
                    CreateDebugWindow();
                }
                else
                {
                    if (_debuggerWindow != null)
                        _debuggerWindow.Close();
                }

                NotifyPropertyChanged("DebuggerIsVisible");
                }                    
    
        }

        private void CreateDebugWindow()
        {
            if (_debuggerWindow == null)
            {
                _debuggerWindow = new Debugger();
                _debuggerWindow.Closing += new CancelEventHandler(_debuggerWindow_Closing);
            }
            if (debugger == null)
                debugger = new DebuggerVM(FrontPanel.Target);

            _debuggerWindow.DataContext = debugger;
            _debuggerWindow.Visibility = System.Windows.Visibility.Visible;
        }

        void _debuggerWindow_Closing(object sender, CancelEventArgs e)
        {
            _debuggerWindow = null;
            _debuggerIsVisible = false;
            NotifyPropertyChanged("DebuggerIsVisible");
        }

        private bool _PPUViewerIsVisible;

        public bool PPUViewerIsVisible
        {
            get { return _PPUViewerIsVisible; }
            set
            {
                _PPUViewerIsVisible = value;
                if (_PPUViewerIsVisible)
                {
                    CreatePPUViewerWindow();
                }
                else
                {
                    if (_ppuViewer != null)
                        _ppuViewer.Close();
                }

                NotifyPropertyChanged("PPUViewerIsVisible");
            }
        }

        private void CreatePPUViewerWindow()
        {
            if (_ppuViewer == null)
            {
                _ppuViewer = new PPUViewer();
                _ppuViewer.Closing += new CancelEventHandler(_ppuViewer_Closing);
            }
            if (debugger == null)
                debugger = new DebuggerVM(FrontPanel.Target);

            _ppuViewer.DataContext = debugger;
            _ppuViewer.Visibility = System.Windows.Visibility.Visible;
        }

        void _ppuViewer_Closing(object sender, CancelEventArgs e)
        {
            _ppuViewer = null;
            _PPUViewerIsVisible = false;
            NotifyPropertyChanged("PPUViewerIsVisible");
        }

        GameInfo _gameInfo =new GameInfo();
        private void ShowCartInfoPopUp()
        {
            _gameInfo.DataContext = FrontPanel.CartInfo;

            _gameInfo.Visibility = System.Windows.Visibility.Visible;
        }

        private bool _ppuDebuggerIsVisible;

        public bool PPUDebuggerIsVisible
        {
            get { return _ppuDebuggerIsVisible && _isDebugging; }
            set { _ppuDebuggerIsVisible = value;
            NotifyPropertyChanged("PPUDebuggerIsVisible");

            }
        }


        #region INotifyPropertyChanged Members

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

        #region IDisposable Members

        public void Dispose()
        {
            FrontPanel.Dispose();
        }

        #endregion
    }
}
