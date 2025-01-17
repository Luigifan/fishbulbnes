
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
			Console.WriteLine("FrontPanel.ctor");
			
			this.Build();
		}
		private IViewModel model;
		
		public IViewModel DataContext 
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

                this.checkPause.CreateBinding("Active", model, "Paused");
				
				this.checkPause.CreateBinding("Value", model, "Paused");
                this.CreateBinding("BufferText", model, "CartInfo");
                
			}
		}

        public string BufferText
        {
            get { return txtRomInfo.Buffer.Text; }
            set { txtRomInfo.Buffer.Text = value; }
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
