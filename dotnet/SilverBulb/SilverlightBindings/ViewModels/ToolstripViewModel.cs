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
using InstiBulb;
using System.Windows.Threading;
using SilverlightCommonUI.ControlPanel;
using SilverlightCommonUI.ScriptViews;
using FishBulb;

namespace SilverlightBindings.ViewModels
{
    public class ToolstripViewModel
    {

        IUnityContainer container;
        Dispatcher dispatcher;
        PlatformDelegates delegates ;

        public ToolstripViewModel(IUnityContainer container, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            delegates = new PlatformDelegates(dispatcher);
            this.container = this.RegisterTools(container);
        }


        IUnityContainer RegisterTools(IUnityContainer container)
        {
            container.RegisterInstance<SRAMWriterDelegate>(delegates.WriteSRAM, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMReaderDelegate>(delegates.ReadSRAM, new ContainerControlledLifetimeManager());

            container.RegisterInstance<System.Windows.Threading.Dispatcher>(dispatcher);

            container.RegisterInstance<IPlatformDelegates>(delegates);

            
            container.RegisterType<IViewModel, SilverlightControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager(), 
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Dispatcher", new ResolvedParameter<Dispatcher>())
                );
            container.RegisterType<FrameworkElement, ControlPanel>("ControlPanel", 
                new InjectionProperty("DataContext", new ResolvedParameter<IViewModel>("ControlPanel")));

            container.RegisterType<IViewModel, fishbulbcommonui.SoundViewModel>("SoundPanel", new ContainerControlledLifetimeManager(), 
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Streamer", new ResolvedParameter<IWavStreamer>()),
                new InjectionProperty("Dispatcher", new ResolvedParameter<Dispatcher>())
                );
            container.RegisterType<FrameworkElement, SoundControls>("SoundPanel",
                new InjectionProperty("DataContext", new ResolvedParameter<IViewModel>("SoundPanel"))
                );

            container.RegisterType<IViewModel, DebuggerVM>("DebuggerVM", 
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Dispatcher", new ResolvedParameter<Dispatcher>())
                );

            container.RegisterType<IViewModel, PatternTablesViewModel>("PatternTableVM",
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Dispatcher", new ResolvedParameter<Dispatcher>())
                );

            container.RegisterType<FrameworkElement, MachineStatus>("CPUPanel",
                new InjectionProperty("DataContext", new ResolvedParameter<IViewModel>("DebuggerVM"))
                );

            container.RegisterType<FrameworkElement, PatternTableViewer>("PPUViewer",
                new InjectionProperty("DataContext", new ResolvedParameter<IViewModel>("PatternTableVM"))
                );

            container.RegisterType<FrameworkElement, About>("AboutPanel");


            // script facing objects
            
            container.RegisterType<IViewModel, ScriptControlPanelVM>("ScriptControlPanel", new ContainerControlledLifetimeManager(), 
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Dispatcher", new ResolvedParameter<Dispatcher>())
                );
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

        public void FillBumpout(ContentControl control, object window)
        {
            string s = window as string;
            if (s == null) return;
            FrameworkElement panel = container.Resolve<FrameworkElement>(s);
            control.Content = panel;
        }

    }
}
