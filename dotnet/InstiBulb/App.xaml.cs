﻿using System;
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
using SlimDXBindings;
using NES.Sound;
using GtkNes;
using InstiBulb.Integration;


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
            
            this.Resources.Add("Container", new NesContainerFactory().RegisterNesTypes(container));
            
        }
    }
}