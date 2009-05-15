
using System;
using System.ComponentModel;
using Gtk;
using Fishbulb.Common.UI;

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
			(model as SoundViewModel).Volume = (float)volScale.Value / 100f;
			// model
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
				volScale.Value = (model as SoundViewModel).Volume * 100;
				break;
			}
		}
	}
	
}
