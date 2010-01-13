using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;
using GtkNes;

namespace fishbulbcommonui.Unity
{
    public static class CommonUIRegisterer
    {
        public static IUnityContainer RegisterCommonUITypes(this IUnityContainer container)
        {
            container.RegisterType<ControlPanelVM>();
            container.RegisterType<SoundViewModel>();
            container.RegisterType<IViewModel, SoundViewModel>("SoundVM", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager());

            return container;
        }
    }
}
