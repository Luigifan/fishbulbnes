using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;
using fishbulbcommonui.SaveStates;
using NES.CPU.nitenedo;

namespace fishbulbcommonui.Unity
{
    public static class CommonUIRegisterer
    {
        public static IUnityContainer RegisterCommonUITypes(this IUnityContainer container)
        {
            //container.RegisterType<IViewModel, SoundViewModel>("SoundVM", new ContainerControlledLifetimeManager(),
            //    new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, SoundViewModel>("SoundPanel", new ContainerControlledLifetimeManager(),
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));

            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            //container.RegisterType<IViewModel, WinDebuggerVM>("DebuggerVM", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, SaveStateVM>("SaveStatePanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            
            return container;
        }
    }
}
