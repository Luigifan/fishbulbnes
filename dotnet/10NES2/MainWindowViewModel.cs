using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using InstiBulb.Commands;
using Microsoft.Practices.Unity;
using NES.CPU.nitenedo;
using System.Windows;
using InstibulbWpfUI;
using InstiBulb;
using Fishbulb.Common.UI;
using InstiBulb.Windowing;
using InstibulbWpfUI.ControlPanel;
using InstiBulb.Views;
using NES.CPU.nitenedo.Interaction;

namespace _10NES2
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        IUnityContainer container;
        NESMachine nes;

        public IUnityContainer Container
        {
            get { return container; }
            set { container = value; }
        }

        public MainWindowViewModel(Window window, string nesType)
        {
            container = new UnityContainer();
            container.RegisterInstance<Window>("MainWindow", window);

            new InstiBulb.Integration.NesContainerFactory().RegisterNesTypes(container, nesType);
            container.RegisterType<FrameworkElement, CartInfoPanel>("CartInfo");
            container.RegisterType<FrameworkElement, ControlPanelView>("ControlPanel");
            container.RegisterType<FrameworkElement, SoundPanelView>("SoundPanel");
            container.RegisterType<FrameworkElement, DebuggerView>("DebugPanel");
            container.RegisterType<FrameworkElement, SaveStateView>("SaveStatePanel");
            container.RegisterType<FrameworkElement, ControllerConfig>("ControllerConfigPanel");

            nes = container.Resolve<NESMachine>();
            nes.RunStatusChangedEvent += new EventHandler<EventArgs>(nes_RunStatusChangedEvent);
            
            showDialogCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowDialog), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            showWindowCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowWindow), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            showBumpOutCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowBumpOut), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            hideBumpOutCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(HideBumpOut), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
        }

        void nes_RunStatusChangedEvent(object sender, EventArgs e)
        {
            switch (nes.RunState)
            {
                case NES.Machine.ControlPanel.RunningStatuses.Running:
                    spinnerWidthFactor = 0.01;
                    break;
                default:
                    userRequestsToolView = false;
                    spinnerWidthFactor = 0.25;
                    break;
            }

            NotifyPropertyChanged("SpinnerWidth");
            NotifyPropertyChanged("ToolsVisible");
        }

        #region Commands and Implementations
        
        ICommand showDialogCommand;
        public ICommand ShowDialogCommand
        {
            get 
            {
                return showDialogCommand;
            }
        }

        ICommand showWindowCommand;
        public ICommand ShowWindowCommand
        {
            get
            {
                return showWindowCommand;
            }
        }

        ICommand showBumpOutCommand;
        public ICommand ShowBumpOutCommand
        {
            get
            {
                return showBumpOutCommand;
            }
        }

        ICommand hideBumpOutCommand;
        public ICommand HideBumpOutCommand
        {
            get
            {
                return hideBumpOutCommand;
            }
        }

        void ShowDialog(object o)
        {

            String s = o as String;
            if (s != null)
            {
                var view = container.Resolve<FrameworkElement>(s);
                view.DataContext = container.Resolve<IViewModel>(s);
                DialogShell w = new DialogShell();
                w.MainGrid.Children.Add(view);
                w.ShowDialog();
            }
        }

        PopupWindowCollection windows = new PopupWindowCollection();

        void ShowWindow(object o)
        {

            String s = o as String;
            if (s != null)
            {

                if (windows.ContainsKey(s))
                {
                    windows[s].Show();
                    windows[s].BringIntoView();
                }
                else
                {

                    var view = container.Resolve<FrameworkElement>(s);
                    view.DataContext = container.Resolve<IViewModel>(s);
                    DialogShell w = new DialogShell();
                    w.MainGrid.Children.Add(view);
                    windows.RegisterWindow(s, w);
                    w.Owner = container.Resolve<Window>("MainWindow");
                    w.Show();
                }
            }
        }

        void ShowBumpOut(object o)
        {
            String s = o as String;
            if (s != null)
            {
                if (s == "DisplaySettings")
                {
                    BumpOut = container.Resolve<IDisplayContext>().PropertiesPanel as FrameworkElement;
                    if (BumpOut == null)
                    {
                        BumpOutVisibility = false;
                    }
                    else
                    {
                        BumpOut.DataContext = container.Resolve<IDisplayContext>();
                        BumpOutVisibility = true;
                    }
                }
                else
                {
                    BumpOut = container.Resolve<FrameworkElement>(s);
                    BumpOut.DataContext = container.Resolve<IViewModel>(s);
                    BumpOutVisibility = true;
                }
                NotifyPropertyChanged("BumpOutVisibility");
                NotifyPropertyChanged("BumpOut");
                
            }
        }

        void HideBumpOut(object o)
        {
            BumpOut = null;    
            BumpOutVisibility = false;
            NotifyPropertyChanged("BumpOutVisibility");
            NotifyPropertyChanged("BumpOut");
        }


        bool CanShowDialog(object o)
        {
            return true;
        }

        double winWidth;

        public double WindowWidth
        {
            get { return winWidth; }
            set
            {
             winWidth= value;
             NotifyPropertyChanged("SpinnerWidth");
             NotifyPropertyChanged("ToolsVisible");
            }
        }

        const double spinnerMaxWidth = 0.25;
        

        double spinnerWidthFactor = 0.25;
        public double SpinnerWidth
        {
            get {
                if (userRequestsToolView)
                    return winWidth * spinnerMaxWidth;

                return winWidth * spinnerWidthFactor; 
            }
        }

        bool userRequestsToolView = true;

        public bool UserRequestsToolView
        {
            get { return userRequestsToolView; }
            set { userRequestsToolView = value;
            NotifyPropertyChanged("SpinnerWidth");
            NotifyPropertyChanged("ToolsVisible");
            }
        }



        #endregion

        public IDisplayContext NESDisplayContext
        {
            get
            {
                return container.Resolve<IDisplayContext>();
            }
        }

        public NESMachine TargetNES
        {
            get
            {
                return nes;
            }
        }

        public FrameworkElement BumpOut
        {
            get;
            set;
        }


        public bool BumpOutVisibility
        {
            get;
            set;
        }


        public bool ToolsVisible
        {
            get
            {
                if (userRequestsToolView)
                    return true;

                return spinnerWidthFactor == spinnerMaxWidth; 
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string s)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(s));
        }

        public void Dispose()
        {
            //foreach (Window w in windows.Values)
            //{
            //    w.Close();
            //}
            windows = null;
            nes.ThreadStoptendo();
            container.Dispose();
        }
    }
}
