using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;

namespace InstibulbWpfUI
{
    static class TypeRegisterer
    {
        public static IUnityContainer RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<EmbeddableUserControl, ControlPanelView>("ControlPanel", new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));

            return container;
        }
    }
}
