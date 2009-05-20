using Fishbulb.Common.UI;
using UIComposition;
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
            this.btnStep.Pressed += new EventHandler(btnStep_Pressed);
        }

        void btnStep_Pressed(object sender, EventArgs e)
        {
            model.ExecuteCommand("Step", null);
        }
	}
}
