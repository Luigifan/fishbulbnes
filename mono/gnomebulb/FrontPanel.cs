
using System;
using Fishbulb.Common.UI;
using UIComposition;

namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class FrontPanel : Gtk.Bin, IBindableElement
	{
		
		public FrontPanel()
		{
			this.Build();
		}
		private IProfileViewModel model;
		
		public IProfileViewModel DataContext 
		{
			get { return model; }
			set { model = value; 
				this.powerButton.CreateBinding("Label", model, "PowerStatusText");
			}
		}

		
	}
	

}
