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

        public MainWindowViewModel(Window window)
        {
            container = new UnityContainer();
            container.RegisterInstance<Window>("MainWindow", window);

            new InstiBulb.Integration.NesContainerFactory().RegisterNesTypes(container, "soft");
            container.RegisterType<FrameworkElement, CartInfoPanel>("CartInfo");
            container.RegisterType<FrameworkElement, ControlPanelView>("ControlPanel");
            container.RegisterType<FrameworkElement, SoundPanelView>("SoundPanel");
            container.RegisterType<FrameworkElement, DebuggerView>("DebugPanel");
            container.RegisterType<FrameworkElement, SaveStateView>("SaveStatePanel");

            nes = container.Resolve<NESMachine>();
            
            showDialogCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowDialog), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            showWindowCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowWindow), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            showBumpOutCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowBumpOut), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            hideBumpOutCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(HideBumpOut), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
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
                    w.Show();
                }
            }
        }

        void ShowBumpOut(object o)
        {
            String s = o as String;
            if (s != null)
            {
                BumpOut = container.Resolve<FrameworkElement>(s);
                BumpOut.DataContext = container.Resolve<IViewModel>(s);
                BumpOutVisibility = true;
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


        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string s)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(s));
        }

        public void Dispose()
        {
            nes.ThreadStoptendo();
            container.Dispose();
        }
    }
}
