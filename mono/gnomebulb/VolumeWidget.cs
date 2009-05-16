
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
			this.volScale.ChangeValue += HandleChangeValue;
		}

		void HandleChangeValue(object o, ChangeValueArgs args)
		{
            model.PropertyChanged -= HandlePropertyChanged;
            this.UpdateSourceBinding(model, (float)volScale.Value / 100f, "Volume");
            model.PropertyChanged += HandlePropertyChanged;
		}
		
		private IProfileViewModel model;
		
		public IProfileViewModel DataContext 
		{
			get { return model; }
			set { model = value; 
				model.PropertyChanged += HandlePropertyChanged;
			}
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "Volume":
                    double? d = this.GetBindingValue(model, "Volume") as double?;
                    if (d!= null)
                        volScale.Value = (double) d;
				break;
			}
		}
	}
	
}
