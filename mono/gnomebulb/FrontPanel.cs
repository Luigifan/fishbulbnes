
using System;
using Fishbulb.Common.UI;
using UIComposition;
using Gtk;

namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class FrontPanel : Gtk.Bin, IBindableElement
	{
        ICommandWrapper loadRom;
        ICommandWrapper powerToggle;
		
		public FrontPanel()
		{
			this.Build();
		}
		private IProfileViewModel model;
		
		public IProfileViewModel DataContext 
		{
			get { return model; }
			set 
            { 
                model = value;
                if (model.Commands.ContainsKey("LoadRom"))
                {
                    loadRom = model.Commands["LoadRom"];
                }
                if (model.Commands.ContainsKey("PowerToggle"))
                {
                    powerToggle = model.Commands["PowerToggle"];
                }
                this.ejectButton.CreateBinding("Label", model, "CurrentCartName");
                this.ejectButton.Clicked +=LoadRomClickEvent;
				this.powerButton.CreateBinding("Label", model, "PowerStatusText");
                this.powerButton.Clicked += new EventHandler(powerButton_Clicked);
			}
		}

        void powerButton_Clicked(object sender, EventArgs e)
        {
            powerToggle.Execute(null);
        }


        void LoadRomClickEvent(object o, EventArgs args)
        {

            Console.WriteLine("LoadRomCLicked");
            if (loadRom != null)
            {
                FileChooserDialog d = new FileChooserDialog("Load ROM", null, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
                d.Filter = new FileFilter();
                d.Filter.AddPattern("*.nes");
                d.Filter.AddPattern("*.zip");
                if (d.Run() == (int)ResponseType.Accept)
                {
                    loadRom.Execute(d.Filename);
                }
                d.Destroy();
            }
        }

		
	}
	

}
