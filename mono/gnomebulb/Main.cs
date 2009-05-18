using System;
using Gtk;
using Microsoft.Practices.Unity;
using GtkNes;

namespace testproject
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            IUnityContainer container = new UnityContainer();

            container.RegisterType<Widget, VolumeWidget>("SoundView");
			container.RegisterType<Widget, FrontPanel>("FrontPanel");
            container.RegisterType<Widget, CheatView>("CheatPanel");
			Application.Init ();
			MainWindow win = new MainWindow (container);
			win.Show ();
			Application.Run ();
		}
	}
}