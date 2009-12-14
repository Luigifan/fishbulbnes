using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;
using System.Windows;
using InstiBulb;
using InstiBulb.WinViewModels;

namespace InstibulbWpfUI
{
    public static class TypeRegisterer
    {
        public static IUnityContainer RegisterWpfUITypes(this IUnityContainer container)
        {
            // wpf specific view models
            container.RegisterType<IViewModel, WinCheatPanelVM>("CheatVM", new ContainerControlledLifetimeManager());

            container.RegisterType<EmbeddableUserControl, ControlPanelHolder>("ControlPanelHolder", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));

            // the visualses
            container.RegisterType<FrameworkElement, ControlPanelView>("ControlPanel", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));
            container.RegisterType<FrameworkElement, SoundPanelView>("SoundPanel", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "SoundVM")));
            container.RegisterType<FrameworkElement, CheatControl>("CheatPanel", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "CheatVM")));
            container.RegisterType<FrameworkElement, CartInfoPanel>("CartInfo", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));
            container.RegisterType<FrameworkElement, MachineStatus>("MachineStats", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            
            //container.RegisterType<ControllerConfig>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(WpfKeyConfigVM), "KeyConfigVM")));
            //container.RegisterType<InstructionRolloutControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            //container.RegisterType<InstructionHistoryControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            //container.RegisterType<NameTableViewerControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            //container.RegisterType<PatternViewerControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            //container.RegisterType<SaveStateView>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "SaveStateVM")));
            //container.RegisterType<TileEditor>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));


            //container.RegisterType<IViewModel, WinDebuggerVM>("DebuggerVM", new ContainerControlledLifetimeManager());
            //container.RegisterType<IViewModel, SaveStateVM>("SaveStateVM", new ContainerControlledLifetimeManager());
            //container.RegisterType<WpfKeyConfigVM>("KeyConfigVM", new ContainerControlledLifetimeManager());

            return container;
        }
    }
}
