using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;

namespace InstibulbWpfUI
{
    public static class TypeRegisterer
    {
        public static IUnityContainer RegisterWpfUITypes(this IUnityContainer container)
        {
            container.RegisterType<EmbeddableUserControl, ControlPanelView>("ControlPanel", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));
            
            // wpf specific inherited view model
            //container.RegisterType<WinCheatPanelVM>();
            //container.RegisterType<WinDebuggerVM>();
            //container.RegisterType<IViewModel, WinCheatPanelVM>("CheatVM", new ContainerControlledLifetimeManager());
            //container.RegisterType<IViewModel, WinDebuggerVM>("DebuggerVM", new ContainerControlledLifetimeManager());
            //container.RegisterType<IViewModel, SaveStateVM>("SaveStateVM", new ContainerControlledLifetimeManager());
            //container.RegisterType<WpfKeyConfigVM>("KeyConfigVM", new ContainerControlledLifetimeManager());

            return container;
        }
    }
}
