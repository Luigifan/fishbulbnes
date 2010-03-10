using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using SilverlightBindings.Views;
using Fishbulb.Common.UI;
using SilverBulb;
using NES.CPU.nitenedo;
using NES.Sound;

namespace SilverlightBindings.ViewModels
{
    public class ToolstripViewModel
    {

        PlatformDelegates delegates = new PlatformDelegates();

        IUnityContainer container;

        public ToolstripViewModel(IUnityContainer container)
        {
            this.container = this.RegisterTools(container);
        }


        IUnityContainer RegisterTools(IUnityContainer container)
        {
            container.RegisterInstance<Fishbulb.Common.UI.GetFileDelegate>(delegates.BrowseForFile);
            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<FrameworkElement, ControlPanel>("ControlPanel", 
                new InjectionProperty("DataContext", new ResolvedParameter<IViewModel>("ControlPanel")));

            container.RegisterType<IViewModel, fishbulbcommonui.SoundViewModel>("SoundPanel", new ContainerControlledLifetimeManager(), 
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Streamer", new ResolvedParameter<IWavStreamer>())
                );
            container.RegisterType<FrameworkElement, SoundControls>("SoundPanel",
                new InjectionProperty("DataContext", new ResolvedParameter<IViewModel>("SoundPanel"))
                );

            container.RegisterType<FrameworkElement, About>("AboutPanel");


            return container;
        }

        public void ShowWindow(object window)
        {
            string s = window as string;
            if (s == null) return;
            ToolPopout popout = new ToolPopout();
            FrameworkElement panel = container.Resolve<FrameworkElement>(s);
            popout.LayoutRoot.Children.Add(panel);
            popout.Show();
        }

    }
}
