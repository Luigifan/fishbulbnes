using System;
using Gtk;
using Microsoft.Practices.Unity;
using GtkNes;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using InstiBulb.Integration;

namespace testproject
{
	class MainClass
	{
		public static void Main (string[] args)
		{
	        NesContainerFactory fact = new NesContainerFactory();
            IUnityContainer container = new UnityContainer();
			
			fact.RegisterNesTypes(container, "soft");

			Application.Init ();

            MainWindow win = new MainWindow (container);
			win.Show ();
			
            Application.Run ();
            
		}
	}
}