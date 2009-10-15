using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using NES.CPU.Fastendo;
using NES.CPU.nitenedo;
using Fishbulb.Common.UI;
using NES.CPU.PPUClasses;
using NES.CPU.Machine.BeepsBoops;
using WpfNESViewer;
using NES.CPU.nitenedo.Interaction;
using NES.CPU.Machine;
using SlimDXBindings;
using NES.Sound;
using GtkNes;
using InstiBulb.Integration;


namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        IUnityContainer container = new UnityContainer();


        protected override void OnStartup(StartupEventArgs e)
        {
            container.RegisterType<MainWindow>(new ContainerControlledLifetimeManager());
            //container.RegisterType<DependencyObject, MainWindow>("MainWindow", new ContainerControlledLifetimeManager());

            this.Resources.Add("Container", new NesContainerFactory().RegisterNesTypes(container));
            base.OnStartup(e);

            container.Resolve<MainWindow>().Initialize().Show();
        }


        void App_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            container.Dispose();
            
        }
    }
}
