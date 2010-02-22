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
using NES.Sound;
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
            var newContainer = new NesContainerFactory().RegisterNesTypes(container);


            this.Resources.Add("Container", container);
            base.OnStartup(e);

            MainWindow win = container.Resolve<MainWindow>();
            win.Initialize();

            win.Show();
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
