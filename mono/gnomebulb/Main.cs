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

			Application.Init ();
			MainWindow win = new MainWindow (container);
			win.Show ();
			Application.Run ();
		}
	}
}