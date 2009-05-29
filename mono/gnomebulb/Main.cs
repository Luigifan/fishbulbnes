using System;
using Gtk;
using Microsoft.Practices.Unity;
using GtkNes;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace testproject
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            IUnityContainer container = new UnityContainer();

            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Containers.Default.Configure(container);

			Application.Init ();

            MainWindow win = new MainWindow (container);
			win.Show ();
			
            Application.Run ();
            
		}
	}
}