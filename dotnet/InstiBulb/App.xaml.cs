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
using WPFamicom.Input;


namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IUnityContainer container = new UnityContainer();
            // register types needed to build a NES
            container.RegisterType<PixelWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, PixelWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<TileDoodler>(new ContainerControlledLifetimeManager());
            container.RegisterType<WavSharer>(new ContainerControlledLifetimeManager() );
            container.Configure<InjectedMembers>().ConfigureInjectionFor<WavSharer>(new InjectionConstructor((float) 44100.0));
            container.RegisterType<Bopper>(new ContainerControlledLifetimeManager());

            container.RegisterType<IControlPad, SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());
            container.RegisterType<InputHandler>(new ContainerControlledLifetimeManager());
            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());
            

            container.RegisterType<NESMachine>("NESMachine", new ContainerControlledLifetimeManager());

            // register views
            container.RegisterType<ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager());
            
            this.Resources.Add("Container", container);

            
            
        }
    }
}
