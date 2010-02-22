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

namespace _10NES2
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        IUnityContainer container;

        public IUnityContainer Container
        {
            get { return container; }
            set { container = value; }
        }
        public MainWindowViewModel()
        {
            container = new UnityContainer();
            new InstiBulb.Integration.NesContainerFactory().RegisterNesTypes(container);
            container.RegisterType<FrameworkElement, CartInfoPanel>("CartInfo");
            container.RegisterType<FrameworkElement, ControlPanelView>("ControlPanel");
            container.RegisterType<FrameworkElement, SoundPanelView>("SoundPanel");
            container.RegisterType<FrameworkElement, MachineStatus>("DebugPanel");



            container.Resolve<NESMachine>();
            
            showDialogCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowDialog), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
            
            showWindowCommand = new DelegateCommand(new Fishbulb.Common.UI.CommandExecuteHandler(ShowWindow), new Fishbulb.Common.UI.CommandCanExecuteHandler(CanShowDialog));
        }
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

        bool CanShowDialog(object o)
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
