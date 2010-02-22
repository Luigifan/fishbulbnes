using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using InstiBulb.Integration;
using NES.CPU.nitenedo;

namespace _10NES2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            MainWindowViewModel vm = new MainWindowViewModel();
            _10NES2.MainWindow win = new MainWindow();
            win.nesDisplay.Target = vm.Container.Resolve<NESMachine>();
            win.nesDisplay.Context = vm.Container.Resolve<NES.CPU.nitenedo.Interaction.IDisplayContext>();

//            container.RegisterInstance<NESDisplay>(new ContainerControlledLifetimeManager(),
//    new InjectionProperty("Target", new ResolvedParameter<NESMachine>()),
//    new InjectionProperty("Context", new ResolvedParameter<IDisplayContext>())
//);
            win.DataContext = vm;
            win.Show();
            

        }
    }
}
