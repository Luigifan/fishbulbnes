using Fishbulb.Common.UI;
using System;
using UIComposition;

namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class AsmViewer : Gtk.Bin, IBindableElement
	{

		#region IBindableElement implementation
		private IProfileViewModel model;
		public Fishbulb.Common.UI.IProfileViewModel DataContext {
			get {
				return model;
			}
			set {
				model=value;
				this.treeview1.CreateBinding("List",model,"DataModel");
			}
		}
		#endregion
		
		public AsmViewer()
		{
			this.Build();
		}
	}
}
