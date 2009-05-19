
using System;
using Fishbulb.Common.UI;
using UIComposition;
namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DebuggerCPUStatusView : Gtk.Bin, IBindableElement
	{
		private IViewModel model;

		#region IBindableElement implementation
		public Fishbulb.Common.UI.IViewModel DataContext {
			get {
				return model;
			}
			set {
				model = value;
				this.lblAccumulator.CreateBinding("Text", model.DataModel, "Accumulator");
			}
		}
		#endregion
		
		public DebuggerCPUStatusView()
		{
			this.Build();
		}
	}
}
