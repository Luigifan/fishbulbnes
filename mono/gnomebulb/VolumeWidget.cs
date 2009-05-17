
using System;
using System.ComponentModel;
using Gtk;
using Fishbulb.Common.UI;
using UIComposition;

namespace GtkNes
{
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class VolumeWidget : Gtk.Bin, IBindableElement
	{
		
		public VolumeWidget()
		{
			this.Build();
			// this.volScale.ChangeValue += HandleChangeValue;
		}

		private IProfileViewModel model;
		
		public IProfileViewModel DataContext 
		{
			get { return model; }
			set { model = value; 
                this.volScale.CreateBinding("Value", model, "Volume");

			}
		}

	}
	
}
