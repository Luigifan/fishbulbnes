using Fishbulb.Common.UI;
using System;

namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DebugStepper : Gtk.Bin, IBindableElement
	{

		
		IViewModel model;
		#region IBindableElement implementation
		public IViewModel DataContext {
			get {
				return model;
			}
			set {
				model=value;
			}
		}
		#endregion
		
		public DebugStepper()
		{
			this.Build();
		}
	}
}
