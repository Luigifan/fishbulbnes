using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;
using System.Windows;
using InstiBulb;
using InstiBulb.WinViewModels;
using NES.CPU.nitenedo;

namespace InstibulbWpfUI
{
    public static class TypeRegisterer
    {
        public static IUnityContainer RegisterWpfUITypes(this IUnityContainer container)
        {



            return container;
        }
    }
}
