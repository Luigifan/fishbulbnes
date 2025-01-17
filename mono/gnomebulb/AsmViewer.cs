using Fishbulb.Common.UI;
using System;
using UIComposition;

namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class AsmViewer : Gtk.Bin, IBindableElement
	{

		#region IBindableElement implementation
		private IViewModel model;
		public Fishbulb.Common.UI.IViewModel DataContext {
			get {
				return model;
			}
			set {
				model=value;
				this.treeview1.CreateTreeViewBinding<string>("List", model, "DataModel");
			}
		}
		#endregion
		
		public AsmViewer()
		{
			this.Build();
		}
	}
}
