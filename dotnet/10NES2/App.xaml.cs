using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using InstiBulb.Integration;
using NES.CPU.nitenedo;
using Fishbulb.Common.UI;

namespace _10NES2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string renderMode; // = _10NES2.Properties.Settings.Default.RenderMode;

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            renderMode = _10NES2.Properties.Settings.Default.RenderMode;
            
            try
            {
                MakeWin();
            }
            catch (Exception )
            {
                
                MessageBox.Show("Could not set up display, reverting to software rendering");
                renderMode = "soft";
                //_10NES2.Properties.Settings.Default.Save();
                MakeWin();
            }
            

        }

        void MakeWin()
        {
            
            if (renderMode != "hard" && renderMode != "soft")
            {
                renderMode = "hard";
            }

            _10NES2.MainWindow win = new MainWindow();
            MainWindowViewModel vm = new MainWindowViewModel(win, renderMode);

            win.nesDisplay.Target = vm.Container.Resolve<NESMachine>();
            
            win.nesDisplay.Context = vm.Container.Resolve<NES.CPU.nitenedo.Interaction.IDisplayContext>();
            vm.NESDisplayContext = win.nesDisplay.Context;

            //            container.RegisterInstance<NESDisplay>(new ContainerControlledLifetimeManager(),
            //    new InjectionProperty("Target", new ResolvedParameter<NESMachine>()),
            //    new InjectionProperty("Context", new ResolvedParameter<IDisplayContext>())
            //);
            win.DataContext = vm;
            win.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            e.Handled = true;
        }


    }
}
